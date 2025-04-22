using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Utility;
using ECom.Utility.Interface;
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

            if (wishlistItem == null)
            {
                TempData["error"] = "Item not found";
                return RedirectToAction(nameof(Index));
            }

            MarkAsDeleted(wishlistItem);
            _unitOfWork.Save();

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

            UpdateCartSession(userId);

            TempData["success"] = "Item moved to cart";
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
            HttpContext.Session.SetInt32(SD.WishlistSessionKey, count);
        }

        private void UpdateCartSession(string userId)
        {
            int count = _unitOfWork.ShoppingCarts.GetAll(c => c.ApplicationUserID == userId && !c.IsDeleted).Count();
            HttpContext.Session.SetInt32(SD.ShoppingCartSessionKey, count);
        }

        #endregion
    }
}
