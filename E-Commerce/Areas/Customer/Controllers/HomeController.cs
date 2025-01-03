using System.Diagnostics;
using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
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

            return View(Product);
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
