using ECom.DataAccess.Repository.IRepository;
using ECom.Utility;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.ViewComponents
{
    public class ShoppingCartCountViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public ShoppingCartCountViewComponent(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var Userid = _userService.GetUserId();
            if (Userid is not null)
            {
                if (_userService.GetCartCount() is null)
                {
                    int CartCount = _unitOfWork.ShoppingCarts
                        .GetAll(cart => cart.ApplicationUserID == _userService.GetUserId() && !cart.IsDeleted)
                        .Count();
                    _userService.SetCartCount(CartCount);
                }
                return View(_userService.GetCartCount());
            }
            else
            {
                _userService.ClearCart();
                return View(0);
            }
        }
    }
}
