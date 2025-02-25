using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace E_Commerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork iUnitOfWork, IUserService UserService)
        {
            _logger = logger;
            _unitOfWork = iUnitOfWork;
            _userService = UserService;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> ProductsList =
                _unitOfWork.Product
                .GetAll(includePropertiesList: "Category")
                .Where(data => !data.IsDeleted);

            var Userid = _userService.GetUserId();
            if (Userid is not null)
            {
                int CartCount = _unitOfWork.ShoppingCarts.GetAll(cart => cart.ApplicationUserID == Userid && !cart.IsDeleted).Count();
                HttpContext.Session.SetInt32(SD.ShoppingCartSessionKey, CartCount);
            }

            return View(ProductsList);
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


        //[Authorize]
        //public IActionResult AddtoCart(int ProductID)
        //{
        //    var Userid = _userService.GetUserId();

        //    var ShoppingCart_TableData = _unitOfWork.ShoppingCarts?
        //        .Get(cart => cart.ProductID == ProductID && cart.ApplicationUserID == Userid && !cart.IsDeleted);

        //    var ShoppingCart = new ShoppingCart
        //    {
        //        ApplicationUserID = Userid,
        //        Count = 1,
        //        ProductID = ProductID,
        //        CreateDate = DateTime.Now
        //    };

        //    if (ShoppingCart_TableData is null)
        //    {
        //        _unitOfWork.ShoppingCarts.Add(ShoppingCart);
        //        _unitOfWork.Save();
        //    }
        //    else
        //    {
        //        ShoppingCart_TableData.Count += ShoppingCart.Count;
        //        _unitOfWork.ShoppingCarts.Update(ShoppingCart_TableData);
        //        _unitOfWork.Save();
        //    }

        //    int CartCount = _unitOfWork.ShoppingCarts.GetAll(cart => cart.ApplicationUserID == Userid && !cart.IsDeleted).Count();
        //    HttpContext.Session.SetInt32(SD.ShoppingCartSessionKey, CartCount);

        //    return Json(new { success = true, message = "Product added to cart successfully!" });
        //}



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
