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
using ECom.Utility.Services;
using ECom.Utility.Interface;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        //private readonly RazorPayService _RazorPayService;
        private IRazorPayService _RazorPayService;

        [BindProperty]
        public OrderVM orderVM { get; set; }
        public OrderController(IUnitOfWork UnitOfWork, IRazorPayService razorPayService)
        {
            _UnitOfWork = UnitOfWork;
            _RazorPayService = razorPayService;
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
            else if (UserID is not null && !User.IsInRole(SD.Role_Company))
            {
                orderHeaders = _UnitOfWork
                .OrderHeaders
                .GetAll(orderData => !orderData.IsDeleted && orderData.PaymentStatus == SD.Payment_Status_Approved
                    && orderData.ApplicationUserID == UserID, includePropertiesList: "_ApplicationUser")
                .ToList().OrderByDescending(data => data.CreateDate);
            }
            else if (User.IsInRole(SD.Role_Company))
            {
                orderHeaders = _UnitOfWork
                    .OrderHeaders
                    .GetAll(orderData => !orderData.IsDeleted && orderData.ApplicationUserID == UserID,
                    includePropertiesList: "_ApplicationUser")
                    .ToList().OrderByDescending(data => data.CreateDate); ;
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


        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        public IActionResult StartProcessing()
        {
            int OrderHeaderID = orderVM.orderHeader.ID;
            _UnitOfWork.OrderHeaders.UpdateStatus(OrderHeaderID, SD.Status_In_Process);
            _UnitOfWork.Save();
            TempData["success"] = "Order Update Sucessfully";
            return RedirectToAction(nameof(Details), new { OrderId = OrderHeaderID });
        }


        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        public IActionResult ShipOrder()
        {
            int OrderHeaderID = orderVM.orderHeader.ID;
            var OrderHeaderdb = _UnitOfWork.OrderHeaders.Get(data => data.ID == OrderHeaderID);
            OrderHeaderdb.TrackingNumber = orderVM.orderHeader.TrackingNumber;
            OrderHeaderdb.Carrier = orderVM.orderHeader.Carrier;
            OrderHeaderdb.OrderStatus = SD.Status_Shipped;
            OrderHeaderdb.ShippingDate = DateTime.Now;

            OrderHeaderdb.InvoiceNumber = Guid.NewGuid().ToString();
            OrderHeaderdb.InvoiceDate = DateTime.Now;

            if (OrderHeaderdb.PaymentStatus == SD.Payment_Status_Delayed_Payment)
            {
                OrderHeaderdb.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }

            _UnitOfWork.OrderHeaders.Update(OrderHeaderdb);
            _UnitOfWork.Save();
            TempData["success"] = "Order Shipped Sucessfully";
            return RedirectToAction(nameof(Details), new { OrderId = OrderHeaderID });
        }



        [HttpPost]
        [Authorize]
        public IActionResult CancelOrder()
        {
            var OrderHeader = _UnitOfWork.OrderHeaders.Get(data => data.ID == orderVM.orderHeader.ID);
            var OrderPayment = _UnitOfWork.OrderPayments.Get(data => data.OrderID == OrderHeader.ID);

            if (OrderHeader.PaymentStatus == SD.Payment_Status_Approved)
            {
                var Result = _RazorPayService.Refund(OrderHeader.PaymentIntendID, (decimal)OrderPayment.Amount);
                _UnitOfWork.OrderHeaders.UpdateStatus(OrderHeader.ID, SD.Status_Cancelled, SD.Status_Refunded);
            }
            else
            {
                _UnitOfWork.OrderHeaders.UpdateStatus(OrderHeader.ID, SD.Status_Cancelled, SD.Status_Cancelled);
            }

            _UnitOfWork.Save();
            TempData["success"] = "Order Cancelled Sucessfully";
            return RedirectToAction(nameof(Details), new { OrderId = orderVM.orderHeader.ID });
        }

    }
}
