using System.Diagnostics;
using System.Security.Claims;
using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork iUnitOfWork)
        {
            _logger = logger;
            _unitOfWork = iUnitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> ProductsList =
                _unitOfWork.Product
                .GetAll(includePropertiesList: "Category")
                .Where(data => !data.IsDeleted);

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
            var UserClaims = (ClaimsIdentity)User.Identity;
            var Userid = UserClaims.FindFirst(ClaimTypes.NameIdentifier).Value;

            var ShoppingCart_TableData = _unitOfWork.ShoppingCarts?
                .Get(cart => cart.ProductID == shoppingCart.ProductID && cart.ApplicationUserID == Userid);

            if (ShoppingCart_TableData is null)
            {
                shoppingCart.ApplicationUserID = Userid;
                _unitOfWork.ShoppingCarts.Add(shoppingCart);
            }
            else
            {
                ShoppingCart_TableData.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCarts.Update(ShoppingCart_TableData);
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
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
