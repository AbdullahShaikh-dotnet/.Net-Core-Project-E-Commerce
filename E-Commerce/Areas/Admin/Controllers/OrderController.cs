using ECom.DataAccess.Repository;
using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        public OrderController(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
        }


        public IActionResult GetAll()
        {
            List<OrderHeader> orderHeaders = _UnitOfWork
                .OrderHeaders
                .GetAll(orderData => !orderData.IsDeleted, includePropertiesList: "ApplicationUsers")
                .ToList();

            return Json(new { data = orderHeaders });
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
