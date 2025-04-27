using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Models.ViewModels;
using ECom.Utility;
using ECom.Utility.Interface;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.Customer.Controllers
{
    [Area(SD.Role_Customer)]
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IMailJetService _mailJetService;

        public WishlistController(IUnitOfWork unitOfWork,
                                   IUserService userService,
                                   IMailJetService mailJetService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _mailJetService = mailJetService;
        }

        public IActionResult Index()
        {
            string userId = _userService.GetUserId();

            var wishlists = _unitOfWork.Wishlist
                .GetAll(w => w.ApplicationUserID == userId && !w.IsDeleted, includePropertiesList: "product")
                .ToList();

            foreach (var item in wishlists)
            {
                var product = item.product;
                product.Category = _unitOfWork.Category.Get(c => c.Id == product.Category_ID);
                product.ProductImages = _unitOfWork.ProductImages.GetAll(img => img.ProductID == product.Id).ToList();
            }

            return View(wishlists);
        }

        public IActionResult AddToWishlist(int productId)
        {
            string userId = _userService.GetUserId();

            var existing = _unitOfWork.Wishlist.Get(w => w.ApplicationUserID == userId && w.ProductID == productId && !w.IsDeleted);
            if (existing != null)
            {
                return Json(new { success = true, message = "Product is already in the wishlist." });
            }

            _unitOfWork.Wishlist.Add(new Wishlist
            {
                ProductID = productId,
                ApplicationUserID = userId
            });
            _unitOfWork.Save();

            UpdateWishlistSession(userId);

            return Json(new { success = true, message = "Product added to wishlist!" });
        }

        public IActionResult RemoveFromWishlist(int itemId)
        {
            var wishlistItem = _unitOfWork.Wishlist.Get(w => w.ID == itemId);
            string userId = _userService.GetUserId();

            if (wishlistItem == null)
            {
                TempData["error"] = "Item not found";
                return RedirectToAction(nameof(Index));
            }

            MarkAsDeleted(wishlistItem);
            _unitOfWork.Save();

            UpdateWishlistSession(userId);

            TempData["success"] = "Item Removed";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult WishlistToCart(int productId)
        {
            string userId = _userService.GetUserId();

            var cartItem = _unitOfWork.ShoppingCarts.Get(c => c.ApplicationUserID == userId && c.ProductID == productId && !c.IsDeleted);
            if (cartItem == null)
            {
                _unitOfWork.ShoppingCarts.Add(new ShoppingCart
                {
                    ApplicationUserID = userId,
                    ProductID = productId,
                    Count = 1,
                    CreateDate = DateTime.Now
                });
            }
            else
            {
                cartItem.Count += 1;
                _unitOfWork.ShoppingCarts.Update(cartItem);
            }

            var wishlistItem = _unitOfWork.Wishlist.Get(w => w.ApplicationUserID == userId && w.ProductID == productId && !w.IsDeleted);
            if (wishlistItem == null)
            {
                TempData["error"] = "Item not found";
                return RedirectToAction(nameof(Index));
            }

            MarkAsDeleted(wishlistItem);
            _unitOfWork.Save();
            UpdateWishlistSession(userId);
            UpdateCartSession(userId);

            TempData["success"] = "Item moved to cart";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ReloadWishlistCount(string type)
        {
            return ReloadCount(type);
        }

        public ViewComponentResult ReloadCount(string type)
        {
            return ViewComponent("CountWidget", new { type });
        }

        public IActionResult AddAllToCart()
        {
            var Userid = _userService.GetUserId();
            var wishlistDB = _unitOfWork.Wishlist.GetAll(data => data.ApplicationUserID == Userid && !data.IsDeleted).ToList();

            foreach (var wishlistItem in wishlistDB)
            {
                var ShoppingListDBUpdate = _unitOfWork.ShoppingCarts
                    .Get(data => data.ProductID == wishlistItem.ProductID && data.ApplicationUserID == Userid && !data.IsDeleted);

                if (ShoppingListDBUpdate is null)
                {
                    _unitOfWork.ShoppingCarts.Add(new ShoppingCart
                    {
                        ApplicationUserID = Userid,
                        ProductID = wishlistItem.ProductID,
                        Count = 1,
                        CreateDate = DateTime.Now
                    });
                }

                wishlistItem.IsDeleted = true;
                wishlistItem.DeletedAt = DateTime.Now;

                _unitOfWork.Wishlist.Update(wishlistItem);
            }
            _unitOfWork.Save();

            UpdateWishlistSession(Userid);
            UpdateCartSession(Userid);

            return RedirectToAction(nameof(Index));
        }



        #region Helpers

        private void MarkAsDeleted(Wishlist wishlist)
        {
            wishlist.IsDeleted = true;
            wishlist.DeletedAt = DateTime.Now;
            _unitOfWork.Wishlist.Update(wishlist);
        }

        private void UpdateWishlistSession(string userId)
        {
            int count = _unitOfWork.Wishlist.GetAll(w => w.ApplicationUserID == userId && !w.IsDeleted).Count();
            _userService.SetWishlistCount(count);
        }

        private void UpdateCartSession(string userId)
        {
            int count = _unitOfWork.ShoppingCarts.GetAll(c => c.ApplicationUserID == userId && !c.IsDeleted).Count();
            _userService.SetCartCount(count);
        }

        #endregion
    }
}
