using ECom.DataAccess.Repository;
using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        public OrderController(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<OrderHeader> orderHeaders = _UnitOfWork
                .OrderHeaders
                .GetAll(orderData => !orderData.IsDeleted, includePropertiesList: "_ApplicationUser")
                .ToList();

            return Json(new { data = orderHeaders });
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
