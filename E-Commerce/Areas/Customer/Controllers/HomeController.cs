using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Models.ViewModels;
using ECom.Utility;
using ECom.Utility.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace E_Commerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ICacheService _cacheService;

        [BindProperty]
        public ProductFilterViewModel _ProductFilterViewModel { get; set; }

        public HomeController(ILogger<HomeController> logger, 
            IUnitOfWork iUnitOfWork, 
            IUserService UserService, 
            ICacheService cacheService)
        {
            _logger = logger;
            _unitOfWork = iUnitOfWork;
            _userService = UserService;
            _cacheService = cacheService;
        }

        public async Task<IActionResult> Index(ProductFilterViewModel model)
        {
            //1.API Rate Limiting per user(Max Attempt: SD.RATE_LIMITING_COUNT)
            string key = $"rate_limit:{_userService.GetUserId()}";
            if (await _cacheService.IsRateLimitedAsync(key, SD.RATE_LIMITING_COUNT, TimeSpan.FromSeconds(1)))
                return StatusCode(429, "Too many requests. Try again later.");


            int RecordPerPage = 8; // Define records per page
            IEnumerable<Product> query;

            var cachedProduct = await _cacheService.GetAsync<IEnumerable<Product>>("ProductObject");

            if (cachedProduct == null)
            {
                query = _unitOfWork.Product.GetAll(includePropertiesList: "Category")
                            .Where(data => !data.IsDeleted);

                await _cacheService.SetAsync("ProductObject", query, TimeSpan.FromSeconds(10));
            }
            else
            {
                query = cachedProduct;
            }


            // Apply Filters
            if (model.SelectedCategories != null && model.SelectedCategories.Any())
            {
                query = query.Where(p => model.SelectedCategories.Select(c => c?.ToLower()).Contains(p.Category.Name.ToLower()));
            }

            if (model.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= (double)model.MinPrice.Value);
            }

            if (model.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= (double)model.MaxPrice.Value);
            }

            // Sorting
            if (model.SortBy is not null)
            {
                if (model.SortBy.ToLower() == "priceasc")
                    query = query.OrderBy(p => p.Price);
                else
                    query = query.OrderByDescending(p => p.Price);
            }

            // Pagination Logic
            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalRecords / RecordPerPage);

            model.Products = query.Skip((model.CurrentPage - 1) * RecordPerPage).Take(RecordPerPage).ToList();
            model.TotalPages = totalPages;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") // AJAX Request
            {
                return PartialView("_ProductPartial", model);
            }

            return View(model);
        }


        public IActionResult Details(int ProductID)
        {
            Product Product =
                _unitOfWork.Product
                .Get(data => data.Id == ProductID, includePropertiesList: "Category");

            ShoppingCart shoppingCart = new ShoppingCart()
            {
                product = Product,
                ProductID = ProductID,
                Count = 1
            };

            return View(shoppingCart);
        }


        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var Userid = _userService.GetUserId();

            var ShoppingCart_TableData = _unitOfWork.ShoppingCarts?
                .Get(cart => cart.ProductID == shoppingCart.ProductID && cart.ApplicationUserID == Userid && !cart.IsDeleted);

            if (ShoppingCart_TableData is null)
            {
                shoppingCart.ApplicationUserID = Userid;
                _unitOfWork.ShoppingCarts.Add(shoppingCart);
                _unitOfWork.Save();
            }
            else
            {
                ShoppingCart_TableData.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCarts.Update(ShoppingCart_TableData);
                _unitOfWork.Save();
            }

            int CartCount = _unitOfWork.ShoppingCarts.GetAll(cart => cart.ApplicationUserID == Userid && !cart.IsDeleted).Count();
            HttpContext.Session.SetInt32(SD.ShoppingCartSessionKey, CartCount);

            TempData["success"] = "Product Added to Cart Successfully";

            return RedirectToAction(nameof(Index));
        }


        [Authorize]
        public IActionResult AddtoCart(int ProductID)
        {
            var userId = _userService.GetUserId();

            var shoppingCartItem = _unitOfWork.ShoppingCarts?
                .Get(cart => cart.ProductID == ProductID && cart.ApplicationUserID == userId && !cart.IsDeleted);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCart
                {
                    ApplicationUserID = userId,
                    ProductID = ProductID,
                    Count = 1,
                    CreateDate = DateTime.Now
                };
                _unitOfWork.ShoppingCarts.Add(shoppingCartItem);
            }
            else
            {
                //shoppingCartItem.Count++;  // Simply increment count
                //_unitOfWork.ShoppingCarts.Update(shoppingCartItem);

                return Json(new { success = true, message = "Product already added to cart" });
            }

            _unitOfWork.Save();  // Save only once

            int cartCount = _unitOfWork.ShoppingCarts
                .GetAll(cart => cart.ApplicationUserID == userId && !cart.IsDeleted)
                .Count();
            HttpContext.Session.SetInt32(SD.ShoppingCartSessionKey, cartCount);

            var CartHtml = ViewComponent("ShoppingCartCount");

            return Json(new { success = true, message = "Product added to cart successfully!" });
        }


        [HttpGet]
        public IActionResult GetFiltersData()
        {
            var CategoriesDB = _unitOfWork.Category.GetAll(data => !data.IsDeleted);

            var MaxListPrice = _unitOfWork.Product.GetAll()
                .OrderByDescending(data => data.ListPrice)
                .FirstOrDefault()?.Price;

            var MinListPrice = _unitOfWork.Product.GetAll()
                .OrderBy(data => data.ListPrice)
                .FirstOrDefault()?.Price;

            var data = new
            {
                category = CategoriesDB,
                MaxPrice = MaxListPrice,
                MinPrice = MinListPrice
            };

            return Json(new { success = true, d = data });
        }


        public IActionResult ReloadCartCount()
        {
            return ViewComponent("ShoppingCartCount");
        }


        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
