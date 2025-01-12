using System.Diagnostics;
using System.Security.Claims;
using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Models.ViewModels;
using ECom.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public ShoppingCartVM _shoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                CartFromDB.DeletedAt = DateTime.Now;
                CartFromDB.IsDeleted = true;
            }
            else
            {
                CartFromDB.Count -= 1;
            }

            _unitOfWork.ShoppingCarts.Update(CartFromDB);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult CartRemove(int CartID)
        {
            var CartFromDB = _unitOfWork.ShoppingCarts.Get(data => data.ID == CartID && !data.IsDeleted);
            if (CartFromDB is null) return RedirectToAction(nameof(Index));

            CartFromDB.DeletedAt = DateTime.Now;
            CartFromDB.IsDeleted = true;
            _unitOfWork.ShoppingCarts.Update(CartFromDB);
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
        [ActionName("Summary")]
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
            }
            else // Company User
            {
                _shoppingCartVM.orderHeader.PaymentStatus = SD.Payment_Status_Delayed_Payment;
                _shoppingCartVM.orderHeader.OrderStatus = SD.Status_Approved;
            }

            _unitOfWork.OrderHeaders.Add(_shoppingCartVM.orderHeader);
            _unitOfWork.Save();

            int OrderID = _shoppingCartVM.orderHeader.ID;

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
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(OrderConfirmation), new { OrderID = OrderID, isCustomer = isCustomer });
        }

        public IActionResult OrderConfirmation(int OrderID, bool isCustomer)
        {
            try
            {
                _unitOfWork.OrderHeaders.UpdateStatus(
                        OrderID, SD.Status_Approved,
                        isCustomer ? SD.Payment_Status_Approved : SD.Payment_Status_Delayed_Payment
                    );

                _unitOfWork.OrderHeaders.UpdatePaymentGatewayID(OrderID);
                DeleteCartDataIfSuccessfull();
            }
            catch { }

            return View(OrderID);
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
