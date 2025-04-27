using ECom.DataAccess.Repository.IRepository;
using Mailjet.Client.Resources;
using Mailjet.Client.Resources.SMS;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.ViewComponents
{
    public class CountWidgetViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public CountWidgetViewComponent(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string type)
        {
            var Userid = _userService.GetUserId();
            int? count = 0;

            if (type == "cart") // Cart Count
            {
                if (Userid is null)
                {
                    _userService.ClearCart();
                    return View(0);
                }


                int? CartCountSession = _userService.GetCartCount();
                if (CartCountSession is null)
                {
                    int CartCount = _unitOfWork.ShoppingCarts
                        .GetAll(cart => cart.ApplicationUserID == _userService.GetUserId() && !cart.IsDeleted)
                        .Count();

                    _userService.SetCartCount(CartCount);
                }

                count = _userService.GetCartCount();
            }
            else if (type == "wishlist") // Wishlist Count
            {
                if (Userid is null)
                {
                    _userService.ClearWishlist();
                    return View(0);
                }

                int? WishListCountSession = _userService.GetWishlistCount();
                if (WishListCountSession is null)
                {
                    int Wishlistcount = _unitOfWork.Wishlist.GetAll(w => w.ApplicationUserID == Userid && !w.IsDeleted).Count();
                    _userService.SetWishlistCount(Wishlistcount);
                }

                count = _userService.GetWishlistCount();
            }

            ViewData["Type"] = type;
            return View(count);

        }
    }
}
