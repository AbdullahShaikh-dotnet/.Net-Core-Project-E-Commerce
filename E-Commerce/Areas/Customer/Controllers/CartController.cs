using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Models.ViewModels;
using ECom.Utility;
using ECom.Utility.Interface;
using ECom.Utility.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace E_Commerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IMailJetService _mailJetService;
        private IRazorPayService _RazorPayService;
        private readonly RazorPaySettings _razorPaySettings;


        [BindProperty]
        public PaymentVM _PaymentVM { get; set; }

        [BindProperty]
        public ShoppingCartVM _shoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork,
            IRazorPayService razorPayService,
            IUserService userService,
            IMailJetService mailJetService,
            IOptions<RazorPaySettings> razorPaySettings)
        {
            _unitOfWork = unitOfWork;
            _RazorPayService = razorPayService;
            _userService = userService;
            _mailJetService = mailJetService;
            _razorPaySettings = razorPaySettings.Value;
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
                    Key = _razorPaySettings.PublishableKey,
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
        public async Task<IActionResult> PaymentVerification()
        {
            _PaymentVM.isPaymentSuccessfull = _RazorPayService.VerifyPayment(_PaymentVM.razorpay_order_id, _PaymentVM.razorpay_payment_id,
                _PaymentVM.razorpay_signature);

            var UserID = _userService.GetUserId();

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

                await SendConfirmationMail(_PaymentVM.OrderID);
            }
            return RedirectToAction(nameof(OrderConfirmation), new { isCustomer = User.IsInRole(SD.Role_Customer), OrderID = _PaymentVM.OrderID });
        }


        private async Task SendConfirmationMail(int OrderID)
        {
            var OrderDetailDB = _unitOfWork.OrderDetails.GetAll(data => data.OrderHeaderID == OrderID);
            string ProductTitles = string.Empty;

            foreach (var OrderDetails in OrderDetailDB)
            {
                var ProductListDB = _unitOfWork.Product.Get(data => data.Id == OrderDetails.ProductID);
                ProductTitles += $"<li style='font-size: 14px; color: #374151; margin-bottom: 8px;'>{ProductListDB.Title}</li>";
            }

            string htmlBody = $@"
                    <!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Order Confirmation</title>
                    </head>
                    <body style='font-family: Arial, sans-serif; background-color: #f9fafb; padding: 20px;'>
                        <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1); padding: 24px;'>
                            <!-- Header -->
                            <div style='text-align: center; margin-bottom: 24px;'>
                                <h1 style='font-size: 24px; font-weight: 600; color: #111827;'>?? Order Confirmed!</h1>
                                <p style='font-size: 16px; color: #6b7280;'>Thank you for your purchase. Your order has been successfully confirmed.</p>
                            </div>

                            <!-- Order Details -->
                            <div style='margin-bottom: 24px;'>
                                <h2 style='font-size: 18px; font-weight: 600; color: #111827; margin-bottom: 12px;'>Order Details</h2>
                                <div style='background-color: #f3f4f6; border-radius: 6px; padding: 16px;'>
                                    <p style='font-size: 14px; color: #374151; margin: 0;'>
                                        <strong>Order ID:</strong> <span style='color: #111827;'>{OrderID}</span>
                                    </p>
                                </div>
                            </div>

                            <!-- Product List -->
                            <div style='margin-bottom: 24px;'>
                                <h2 style='font-size: 18px; font-weight: 600; color: #111827; margin-bottom: 12px;'>Products Ordered</h2>
                                <ol style='list-style-type: decimal; padding-left: 20px; margin: 0;'>
                                    {ProductTitles}
                                </ol>
                            </div>

                            <!-- Footer -->
                            <div style='text-align: center; color: #6b7280; font-size: 14px;'>
                                <p>If you have any questions, feel free to <a href='mailto:support@example.com' style='color: #3b82f6; text-decoration: none;'>contact us</a>.</p>
                                <p>© 2023 Your Company. All rights reserved.</p>
                            </div>
                        </div>
                    </body>
                    </html>";

            await _mailJetService.SendEmailAsync(
                "abdullah.goldmedalindia@gmail.com",
                "Order Confirmed",
                htmlBody
            );
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
                Key = _razorPaySettings.PublishableKey,
            };
            return View("Payment", _PaymentVM);
        }

        private void DeleteCartDataIfSuccessfull()
        {
            var UserID = _userService.GetUserId();
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

                _userService.ClearCart();
                _userService.SetCartCount((int)_userService.GetCartCount());
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
