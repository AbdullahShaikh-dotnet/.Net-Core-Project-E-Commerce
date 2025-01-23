using System.Diagnostics;
using System.Security.Claims;
using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Models.ViewModels;
using ECom.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;

namespace E_Commerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private RazorPayService _RazorPayService;

        [BindProperty]
        public PaymentVM _PaymentVM { get; set; }

        [BindProperty]
        public ShoppingCartVM _shoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork, RazorPayService razorPayService, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _RazorPayService = razorPayService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            var UserClaims = (ClaimsIdentity)User.Identity;
            var UserID = UserClaims.FindFirst(ClaimTypes.NameIdentifier).Value;

            _shoppingCartVM = new()
            {
                shoppingCartsList = _unitOfWork.ShoppingCarts
                .GetAll(Sdata => Sdata.ApplicationUserID == UserID && !Sdata.IsDeleted, includePropertiesList: "product"),
                orderHeader = new()
            };

            double CartTotal = 0;
            foreach (var cart in _shoppingCartVM.shoppingCartsList)
            {
                cart.ShoppingCartPrice = GetPriceBasedOnCount(cart);
                CartTotal += (cart.ShoppingCartPrice * cart.Count);
            }
            _shoppingCartVM.orderHeader.OrderTotal = CartTotal;
            return View(_shoppingCartVM);
        }


        public IActionResult CartPlus(int CartID)
        {
            var CartFromDB = _unitOfWork.ShoppingCarts.Get(data => data.ID == CartID && !data.IsDeleted);
            if (CartFromDB is null) return RedirectToAction(nameof(Index));

            CartFromDB.Count += 1;
            _unitOfWork.ShoppingCarts.Update(CartFromDB);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult CartMinus(int CartID)
        {
            var CartFromDB = _unitOfWork.ShoppingCarts.Get(data => data.ID == CartID && !data.IsDeleted);
            if (CartFromDB is null) return RedirectToAction(nameof(Index));

            if (CartFromDB.Count <= 1)
            {
                _unitOfWork.ShoppingCarts.Remove(CartFromDB);
                int CartCount = _unitOfWork.ShoppingCarts
                    .GetAll(cart => cart.ApplicationUserID == _userService.GetUserId() && !cart.IsDeleted)
                    .Count() - 1;
                HttpContext.Session.SetInt32(SD.ShoppingCartSessionKey, CartCount);
            }
            else
            {
                CartFromDB.Count -= 1;
                _unitOfWork.ShoppingCarts.Update(CartFromDB);
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult CartRemove(int CartID)
        {
            var CartFromDB = _unitOfWork.ShoppingCarts.Get(data => data.ID == CartID && !data.IsDeleted);
            if (CartFromDB is null) return RedirectToAction(nameof(Index));
            _unitOfWork.ShoppingCarts.Remove(CartFromDB);

            int CartCount = _unitOfWork.ShoppingCarts
                .GetAll(cart => cart.ApplicationUserID == _userService.GetUserId() && !cart.IsDeleted)
                .Count() - 1;
            HttpContext.Session.SetInt32(SD.ShoppingCartSessionKey, CartCount);

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Summary()
        {

            var UserClaims = (ClaimsIdentity)User.Identity;
            var UserID = UserClaims.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser UserDetails = _unitOfWork.ApplicationUsers.Get(Userdata => Userdata.Id == UserID);

            _shoppingCartVM = new()
            {
                shoppingCartsList = _unitOfWork.ShoppingCarts
                .GetAll(Sdata => Sdata.ApplicationUserID == UserID && !Sdata.IsDeleted, includePropertiesList: "product"),
                orderHeader = new()
            };

            _shoppingCartVM.orderHeader._ApplicationUser = UserDetails;
            _shoppingCartVM.orderHeader.Name = UserDetails.Name;
            _shoppingCartVM.orderHeader.PhoneNumber = UserDetails.PhoneNumber;
            _shoppingCartVM.orderHeader.State = UserDetails.State;
            _shoppingCartVM.orderHeader.City = UserDetails.City;
            _shoppingCartVM.orderHeader.StreetAddress = UserDetails.StreetAddress;
            _shoppingCartVM.orderHeader.PostalCode = UserDetails.PostalCode;

            double CartTotal = 0;
            foreach (var cart in _shoppingCartVM.shoppingCartsList)
            {
                cart.ShoppingCartPrice = GetPriceBasedOnCount(cart);
                CartTotal += (cart.ShoppingCartPrice * cart.Count);
            }
            _shoppingCartVM.orderHeader.OrderTotal = CartTotal;

            return View(_shoppingCartVM);
        }


        [HttpPost]
        [ActionName("PlaceOrder")]
        public IActionResult SummaryPost()
        {
            var UserClaims = (ClaimsIdentity)User.Identity;
            var UserID = UserClaims.FindFirst(ClaimTypes.NameIdentifier).Value;

            _shoppingCartVM.shoppingCartsList = _unitOfWork.ShoppingCarts
                .GetAll(Sdata => Sdata.ApplicationUserID == UserID && !Sdata.IsDeleted, includePropertiesList: "product");

            _shoppingCartVM.orderHeader.ApplicationUserID = UserID;
            _shoppingCartVM.orderHeader.OrderDate = DateTime.Now;

            ApplicationUser applicationUser = _unitOfWork.ApplicationUsers.Get(user => user.Id == UserID);
            applicationUser.PhoneNumber ??= _shoppingCartVM.orderHeader.PhoneNumber;
            applicationUser.StreetAddress ??= _shoppingCartVM.orderHeader.StreetAddress;
            applicationUser.City ??= _shoppingCartVM.orderHeader.City;
            applicationUser.State ??= _shoppingCartVM.orderHeader.State;
            applicationUser.PostalCode ??= _shoppingCartVM.orderHeader.PostalCode;

            _unitOfWork.ApplicationUsers.Update(applicationUser);

            double CartTotal = 0;
            foreach (var cart in _shoppingCartVM.shoppingCartsList)
            {
                cart.ShoppingCartPrice = GetPriceBasedOnCount(cart);
                CartTotal += (cart.ShoppingCartPrice * cart.Count);
            }
            _shoppingCartVM.orderHeader.OrderTotal = CartTotal;

            bool isCustomer = applicationUser.CompanyID.GetValueOrDefault() == 0;
            if (isCustomer)
            {
                _shoppingCartVM.orderHeader.PaymentStatus = SD.Payment_Status_Pending;
                _shoppingCartVM.orderHeader.OrderStatus = SD.Status_Pending;

                _unitOfWork.OrderHeaders.Add(_shoppingCartVM.orderHeader);
                _unitOfWork.Save();


                int OrderID = _shoppingCartVM.orderHeader.ID;
                decimal OrderTotal = (decimal)_shoppingCartVM.orderHeader.OrderTotal;
                var RazorPayOrder = _RazorPayService.CreateOrder(OrderTotal);
                string SessionId = RazorPayOrder["id"]; // OrderID

                _PaymentVM = new()
                {
                    Email = _shoppingCartVM.orderHeader._ApplicationUser.Email,
                    Name = _shoppingCartVM.orderHeader._ApplicationUser.Name,
                    ContactNo = _shoppingCartVM.orderHeader._ApplicationUser.PhoneNumber,
                    Amount = OrderTotal,
                    OrderID = OrderID,
                    razorpay_order_id = SessionId,
                    Description = "Testing Enviroment",
                    Key = _RazorPayService._key,
                };


                foreach (var cart in _shoppingCartVM.shoppingCartsList)
                {
                    OrderDetail Orderdetails = new()
                    {
                        ProductID = cart.ProductID,
                        Price = cart.ShoppingCartPrice,
                        OrderHeaderID = OrderID,
                        Count = cart.Count,
                    };
                    _unitOfWork.OrderDetails.Add(Orderdetails);
                }
                _unitOfWork.Save();
                return View("Payment", _PaymentVM);

            }
            else // Company User
            {
                _shoppingCartVM.orderHeader.PaymentStatus = SD.Payment_Status_Delayed_Payment;
                _shoppingCartVM.orderHeader.OrderStatus = SD.Status_Approved;
                _unitOfWork.OrderHeaders.Add(_shoppingCartVM.orderHeader);


                foreach (var cart in _shoppingCartVM.shoppingCartsList)
                {
                    OrderDetail Orderdetails = new()
                    {
                        ProductID = cart.ProductID,
                        Price = cart.ShoppingCartPrice,
                        OrderHeaderID = _shoppingCartVM.orderHeader.ID,
                        Count = cart.Count,
                    };
                    _unitOfWork.OrderDetails.Add(Orderdetails);
                }
                _unitOfWork.Save();

                return RedirectToAction(nameof(OrderConfirmation), new { isCustomer = false, OrderID = _shoppingCartVM.orderHeader.ID });
            }
        }

        public IActionResult OrderConfirmation(bool isCustomer, int OrderID)
        {
            var UserClaims = (ClaimsIdentity)User.Identity;
            var UserID = UserClaims.FindFirst(ClaimTypes.NameIdentifier).Value;
            var _OrderPayment = _unitOfWork.OrderPayments.Get(data => data.ApplicationUserID == UserID && data.OrderID == OrderID);
            _OrderPayment.isCustomer = isCustomer;

            try
            {
                if (isCustomer)
                {
                    if (_OrderPayment.isPaymentSuccessfull)
                    {
                        _unitOfWork.OrderHeaders.UpdateStatus(
                                _OrderPayment.OrderID, SD.Status_Approved, SD.Payment_Status_Approved
                            );

                        _unitOfWork.OrderHeaders.UpdatePaymentGatewayID(_OrderPayment.OrderID);
                        DeleteCartDataIfSuccessfull();
                        _unitOfWork.Save();

                        return View(_OrderPayment);
                    }
                    else
                    {
                        return View(_OrderPayment);
                    }
                }
                else
                {
                    _unitOfWork.OrderHeaders.UpdateStatus
                        (OrderID, SD.Status_Approved, SD.Payment_Status_Delayed_Payment);

                    DeleteCartDataIfSuccessfull();
                    OrderPayment OrderPaymentObject = new OrderPayment
                    {
                        isPaymentSuccessfull = true,
                        OrderID = OrderID,
                        isCustomer = isCustomer
                    };
                    _unitOfWork.Save();
                    return View(OrderPaymentObject);
                }
            }
            catch { }
            return View(_OrderPayment);
        }


        [HttpPost]
        public IActionResult PaymentVerification()
        {
            _PaymentVM.isPaymentSuccessfull = _RazorPayService.VerifyPayment(_PaymentVM.razorpay_order_id, _PaymentVM.razorpay_payment_id, _PaymentVM.razorpay_signature);
            var UserClaims = (ClaimsIdentity)User.Identity;
            var UserID = UserClaims.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (_PaymentVM.isPaymentSuccessfull)
            {
                var OrderHeaderDb = _unitOfWork.OrderHeaders.Get(data => data.ID == _PaymentVM.OrderID, includePropertiesList: "_ApplicationUser");

                var orderPayment = new OrderPayment
                {
                    ApplicationUserID = UserID,
                    Name = _PaymentVM.Name,
                    Email = _PaymentVM.Email,
                    ContactNo = _PaymentVM.ContactNo,
                    Amount = (double)_PaymentVM.Amount,
                    Description = _PaymentVM.Description,
                    Public_Key = _PaymentVM.Key,
                    isPaymentSuccessfull = _PaymentVM.isPaymentSuccessfull,
                    OrderID = _PaymentVM.OrderID,
                    razorpay_order_id = _PaymentVM.razorpay_order_id,
                    razorpay_payment_id = _PaymentVM.razorpay_payment_id,
                    razorpay_signature = _PaymentVM.razorpay_signature
                };

                _unitOfWork.OrderPayments.Add(orderPayment);
                _unitOfWork.OrderHeaders.UpdatePaymentGatewayID(_PaymentVM.OrderID);
                _unitOfWork.OrderHeaders.UpdateStatus(_PaymentVM.OrderID, OrderHeaderDb.OrderStatus, SD.Payment_Status_Approved);
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(OrderConfirmation), new { isCustomer = User.IsInRole(SD.Role_Customer), OrderID = _PaymentVM.OrderID });
        }


        [HttpPost]
        [ActionName("DelayedPayNow")]
        public IActionResult PayNow(int OrderHeaderID)
        {
            var OrderHeaderDb = _unitOfWork.OrderHeaders.Get(data => data.ID == OrderHeaderID, includePropertiesList: "_ApplicationUser");

            decimal OrderTotal = (decimal)OrderHeaderDb.OrderTotal;
            var RazorPayOrder = _RazorPayService.CreateOrder(OrderTotal);
            string SessionId = RazorPayOrder["id"]; // Razor Pay OrderID

            _PaymentVM = new()
            {
                Email = OrderHeaderDb._ApplicationUser.Email,
                Name = OrderHeaderDb._ApplicationUser.Name,
                ContactNo = OrderHeaderDb._ApplicationUser.PhoneNumber,
                Amount = OrderTotal,
                OrderID = OrderHeaderID,
                razorpay_order_id = SessionId,
                Description = "Testing Enviroment",
                Key = _RazorPayService._key,
            };
            return View("Payment", _PaymentVM);
        }

        private void DeleteCartDataIfSuccessfull()
        {
            ClaimsIdentity UserClaims = (ClaimsIdentity)User.Identity;
            var UserID = UserClaims.FindFirst(ClaimTypes.NameIdentifier).Value;

            try
            {
                List<ShoppingCart> shoppingCarts = _unitOfWork
                    .ShoppingCarts.GetAll(cartData => cartData.ApplicationUserID == UserID && !cartData.IsDeleted)
                    .ToList();

                foreach (var cart in shoppingCarts)
                {
                    cart.IsDeleted = true;
                    cart.DeletedAt = DateTime.Now;
                    cart.isOrderPlaced = true;
                    _unitOfWork.ShoppingCarts.Update(cart);
                }
                _unitOfWork.Save();
            }
            catch { }
        }

        private double GetPriceBasedOnCount(ShoppingCart shoppingCart)
        {
            return shoppingCart.Count switch
            {
                <= 50 => shoppingCart.product.Price,
                <= 100 => shoppingCart.product.Price50,
                _ => shoppingCart.product.Price100
            };
        }
    }
}
