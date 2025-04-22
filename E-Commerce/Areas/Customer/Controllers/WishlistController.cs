using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Models.ViewModels;
using ECom.Utility;
using ECom.Utility.Interface;
using ECom.Utility.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace E_Commerce.Areas.Customer.Controllers
{
    [Area(SD.Role_Customer)]
    [Authorize]
    public class WishlistController : Controller
    {
        [BindProperty]
        public Wishlist _wishlist { get; set; }


        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IMailJetService _mailJetService;


        public WishlistController(IUnitOfWork unitOfWork,
            IRazorPayService razorPayService,
            IUserService userService,
            IMailJetService mailJetService,
            IOptions<RazorPaySettings> razorPaySettings)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _mailJetService = mailJetService;
        }


        public IActionResult Index()
        {
            List<Wishlist> Wishlists = _unitOfWork.Wishlist.GetAll(data => data.ApplicationUserID == _userService.GetUserId(), includePropertiesList: "product").ToList();
            foreach (var Wishlis in Wishlists)
            {
                Wishlis.product.Category = _unitOfWork.Category.Get(data => data.Id == Wishlis.product.Category_ID);
                Wishlis.product.ProductImages = _unitOfWork.ProductImages.GetAll(data => data.ProductID == Wishlis.product.Id).ToList();
            }
            return View(Wishlists);
        }
    }
}
