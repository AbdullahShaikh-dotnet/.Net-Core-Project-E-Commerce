using System.Diagnostics;
using System;
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
        public IActionResult GetAll(string Status)
        {
            IEnumerable<OrderHeader> orderHeaders = _UnitOfWork
                .OrderHeaders
                .GetAll(orderData => !orderData.IsDeleted, includePropertiesList: "_ApplicationUser")
                .ToList();



            switch (Status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(data => data.PaymentStatus == SD.Payment_Status_Delayed_Payment);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(data => data.OrderStatus == SD.Status_In_Process);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(data => data.OrderStatus == SD.Status_Shipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(data => data.OrderStatus == SD.Status_Approved);
                    break;
                default:
                    break;
            }
            return Json(new { data = orderHeaders });
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Details(int OrderId)
        {
            OrderVM orderVM = new()
            {
                orderHeader = _UnitOfWork.OrderHeaders.Get(data => data.ID == OrderId, includePropertiesList: "_ApplicationUser"),
                orderDetails = _UnitOfWork.OrderDetails.GetAll(data => data.OrderHeaderID == OrderId, includePropertiesList: "_Product")
            };
            return View(orderVM);
        }

    }
}
