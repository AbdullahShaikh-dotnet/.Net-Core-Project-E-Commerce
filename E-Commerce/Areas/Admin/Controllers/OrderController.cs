using System.Diagnostics;
using System;
using ECom.DataAccess.Repository;
using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using ECom.Models.ViewModels;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin )]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        [BindProperty]
        public OrderVM orderVM { get; set; }
        public OrderController(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
        }

        [HttpGet]
        public IActionResult GetAll(string Status)
        {
            IEnumerable<OrderHeader> orderHeaders;

            var UserClaims = (ClaimsIdentity)User.Identity;
            var UserID = UserClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orderHeaders = _UnitOfWork
                .OrderHeaders
                .GetAll(orderData => !orderData.IsDeleted, includePropertiesList: "_ApplicationUser")
                .ToList();
            }
            else if(UserID is not null)
            {
                orderHeaders = _UnitOfWork
                .OrderHeaders
                .GetAll(orderData => !orderData.IsDeleted && orderData.ApplicationUserID == UserID, includePropertiesList: "_ApplicationUser")
                .ToList();
            }
            else
            {
                return Json(new { data = new OrderHeader() });
            }

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
            orderVM = new()
            {
                orderHeader = _UnitOfWork.OrderHeaders.Get(data => data.ID == OrderId, includePropertiesList: "_ApplicationUser"),
                orderDetails = _UnitOfWork.OrderDetails.GetAll(data => data.OrderHeaderID == OrderId, includePropertiesList: "_Product")
            };
            return View(orderVM);
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        public IActionResult UpdateOrderDetails()
        {
            var OrderHeaderDB = _UnitOfWork.OrderHeaders.Get(data => data.ID == orderVM.orderHeader.ID);

            OrderHeaderDB.Name = orderVM.orderHeader.Name;
            OrderHeaderDB.PhoneNumber = orderVM.orderHeader.PhoneNumber;
            OrderHeaderDB.StreetAddress = orderVM.orderHeader.StreetAddress;
            OrderHeaderDB.City = orderVM.orderHeader.City;
            OrderHeaderDB.State = orderVM.orderHeader.State;
            OrderHeaderDB.PostalCode = orderVM.orderHeader.PostalCode;

            OrderHeaderDB.Carrier = !string.IsNullOrEmpty(orderVM.orderHeader.Carrier)
                ? orderVM.orderHeader.Carrier : string.Empty;

            OrderHeaderDB.TrackingNumber = !string.IsNullOrEmpty(orderVM.orderHeader.TrackingNumber)
            ? orderVM.orderHeader.TrackingNumber : string.Empty;

            _UnitOfWork.OrderHeaders.Update(OrderHeaderDB);
            _UnitOfWork.Save();

            TempData["success"] = "Order Update Sucessfully";
            return RedirectToAction(nameof(Details), new { OrderId = OrderHeaderDB.ID });
        }

    }
}
