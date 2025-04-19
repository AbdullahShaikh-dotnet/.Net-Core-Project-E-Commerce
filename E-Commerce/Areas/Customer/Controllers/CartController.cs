using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Models.ViewModels;
using ECom.Utility;
using ECom.Utility.Interface;
using ECom.Utility.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Razorpay.Api;
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

            IEnumerable<ProductImages> ProductImages = _unitOfWork.ProductImages.GetAll();

            double CartTotal = 0;
            foreach (var cart in _shoppingCartVM.shoppingCartsList)
            {
                cart.product.ProductImages = ProductImages
                    .Where(data => data.ProductID == cart.ProductID)
                    .ToList();

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

            IEnumerable<ProductImages> ProductImages = _unitOfWork.ProductImages.GetAll();

            double CartTotal = 0;
            foreach (var cart in _shoppingCartVM.shoppingCartsList)
            {
                cart.product.ProductImages = ProductImages
                    .Where(data => data.ProductID == cart.ProductID)
                    .ToList();
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
                _unitOfWork.Save();

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

            try
            {
                if (isCustomer)
                {
                    _OrderPayment.isCustomer = isCustomer;

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
            string ProductItems = string.Empty;

            var UserData = _unitOfWork.ApplicationUsers.Get(data => data.Id == _userService.GetUserId());
            string SendToEmail = UserData.Email;

            decimal orderTotal = 0;
            foreach (var OrderDetails in OrderDetailDB)
            {
                var ProductListDB = _unitOfWork.Product.Get(data => data.Id == OrderDetails.ProductID);
                ProductItems += $@"
                                <tr>
                                    <td style='padding: 12px 0; border-bottom: 1px solid #f3f4f6;'>
                                        <div style='font-size: 16px; color: #111827; font-weight: 500;'>{ProductListDB.Title}</div>
                                        <div style='font-size: 14px; color: #6b7280;'>Qty: {OrderDetails.Count}</div>
                                    </td>
                                    <td style='padding: 12px 0; border-bottom: 1px solid #f3f4f6; text-align: right; font-size: 16px; color: #111827; font-weight: 500;'>
                                        ₹{OrderDetails.Price * OrderDetails.Count:0.00}
                                    </td>
                                </tr>";
                orderTotal += (decimal)OrderDetails.Price * OrderDetails.Count;
            }

            string htmlBody = $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <meta charset='UTF-8'>
                            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                            <title>Order Confirmation</title>
                        </head>
                        <body style='font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f9fafb;'>
                            <!-- Main Container -->
                            <table width='100%' border='0' cellspacing='0' cellpadding='0' style='background-color: #f9fafb;'>
                                <tr>
                                    <td align='center'>
                                        <table width='600' border='0' cellspacing='0' cellpadding='0' style='background-color: #ffffff; margin: 0 auto;'>
                                            <!-- Header -->
                                            <tr>
                                                <td style='padding: 24px; border-bottom: 1px solid #e5e7eb;'>
                                                    <div style='font-size: 20px; font-weight: 600; color: #111827;'>Order Confirmation</div>
                                                    <div style='font-size: 14px; color: #6b7280; margin-top: 4px;'>Thank you for your purchase</div>
                                                </td>
                                            </tr>
                            
                                            <!-- Content -->
                                            <tr>
                                                <td style='padding: 24px;'>
                                                    <!-- Order Info -->
                                                    <table width='100%' border='0' cellspacing='0' cellpadding='0' style='margin-bottom: 24px;'>
                                                        <tr>
                                                            <td style='background-color: #f9fafb; border-radius: 8px; padding: 16px;'>
                                                                <table width='100%' border='0' cellspacing='0' cellpadding='0'>
                                                                    <tr>
                                                                        <td width='120' style='font-size: 14px; color: #6b7280; padding-bottom: 8px;'>Order Number</td>
                                                                        <td style='font-size: 14px; color: #111827; font-weight: 500; padding-bottom: 8px;'>#{OrderID}</td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td width='120' style='font-size: 14px; color: #6b7280;'>Order Date</td>
                                                                        <td style='font-size: 14px; color: #111827; font-weight: 500;'>{DateTime.Now.ToString("MMMM dd, yyyy")}</td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                    
                                                    <!-- Shipping Info -->
                                                    <table width='100%' border='0' cellspacing='0' cellpadding='0' style='margin-bottom: 24px;'>
                                                        <tr>
                                                            <td style='font-size: 18px; color: #111827; font-weight: 600; padding-bottom: 16px;'>Shipping Information</td>
                                                        </tr>
                                                        <tr>
                                                            <td style='background-color: #f9fafb; border-radius: 8px; padding: 16px;'>
                                                                <table width='100%' border='0' cellspacing='0' cellpadding='0'>
                                                                    <tr>
                                                                        <td width='120' style='font-size: 14px; color: #6b7280; padding-bottom: 8px;'>Full Name</td>
                                                                        <td style='font-size: 14px; color: #111827; font-weight: 500; padding-bottom: 8px;'>{ UserData.Name }</td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td width='120' style='font-size: 14px; color: #6b7280; padding-bottom: 8px;'>Phone</td>
                                                                        <td style='font-size: 14px; color: #111827; font-weight: 500; padding-bottom: 8px;'>{ UserData.PhoneNumber }</td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td width='120' style='font-size: 14px; color: #6b7280; padding-bottom: 8px;'>Address</td>
                                                                        <td style='font-size: 14px; color: #111827; font-weight: 500; padding-bottom: 8px;'>{ UserData.StreetAddress }</td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td width='120' style='font-size: 14px; color: #6b7280; padding-bottom: 8px;'>City/State/Zip</td>
                                                                        <td style='font-size: 14px; color: #111827; font-weight: 500; padding-bottom: 8px;'>{ UserData.City }, { UserData.State }, { UserData.PostalCode }</td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                    
                                                    <!-- Order Items -->
                                                    <table width='100%' border='0' cellspacing='0' cellpadding='0' style='margin-bottom: 24px;'>
                                                        <tr>
                                                            <td style='font-size: 18px; color: #111827; font-weight: 600; padding-bottom: 16px;'>Order Summary</td>
                                                        </tr>
                                                        {ProductItems}
                                                    </table>
                                    
                                                    <!-- Order Total -->
                                                    <table width='100%' border='0' cellspacing='0' cellpadding='0' style='margin-bottom: 24px;'>
                                                        <tr>
                                                            <td style='padding: 12px 0; border-bottom: 1px solid #f3f4f6;'>
                                                                <table width='100%' border='0' cellspacing='0' cellpadding='0'>
                                                                    <tr>
                                                                        <td style='color: #6b7280;'>Subtotal</td>
                                                                        <td style='text-align: right; font-weight: 500;'>₹{orderTotal:0.00}</td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style='padding: 12px 0; border-bottom: 1px solid #f3f4f6;'>
                                                                <table width='100%' border='0' cellspacing='0' cellpadding='0'>
                                                                    <tr>
                                                                        <td style='color: #6b7280;'>Shipping</td>
                                                                        <td style='text-align: right; font-weight: 500;'>Free</td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style='padding: 12px 0; border-bottom: 1px solid #f3f4f6;'>
                                                                <table width='100%' border='0' cellspacing='0' cellpadding='0'>
                                                                    <tr>
                                                                        <td style='color: #6b7280;'>Tax</td>
                                                                        <td style='text-align: right; font-weight: 500;'>Included</td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style='padding-top: 16px; border-top: 1px solid #e5e7eb;'>
                                                                <table width='100%' border='0' cellspacing='0' cellpadding='0'>
                                                                    <tr>
                                                                        <td style='font-weight: 600;'>Total</td>
                                                                        <td style='text-align: right; font-weight: 600;'>₹{orderTotal:0.00}</td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                            
                                            <!-- Footer -->
                                            <tr>
                                                <td style='padding: 24px; text-align: center; border-top: 1px solid #e5e7eb;'>
                                                    <div style='font-size: 12px; color: #9ca3af; margin-bottom: 16px;'>
                                                        By placing your order, you agree to our <a href='#' style='color: #6366f1; text-decoration: none;'>Terms of Service</a> and <a href='#' style='color: #6366f1; text-decoration: none;'>Privacy Policy</a>
                                                    </div>
                                                    <div style='font-size: 12px; color: #9ca3af;'>
                                                        © {DateTime.Now.Year} E-commerce. All rights reserved.
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </body>
                        </html>";

            await _mailJetService.SendEmailAsync(
                SendToEmail,
                $"Your Order #{OrderID} is Confirmed",
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
