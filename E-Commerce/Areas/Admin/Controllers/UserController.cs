using E_Commerce.DataAccess.Data;
using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Models.ViewModels;
using ECom.Utility;
using ECom.Utility.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebSocketManager _webSocketManager;

        public UserController(ApplicationDbContext db,
            IWebSocketManager webSocketManager, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _webSocketManager = webSocketManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("admin/user/getall")]
        public IActionResult GetAll()
        {
            List<ApplicationUser> usersList = _db.ApplicationUsers.Include(data => data.Company).ToList();

            var Roles = _db.Roles.ToList();
            var UserRoles = _db.UserRoles.ToList();

            foreach (var user in usersList)
            {
                var UserRoleID = UserRoles.FirstOrDefault(data => data.UserId == user.Id).RoleId;
                user.Role = Roles.FirstOrDefault(data => data.Id == UserRoleID).Name;

                if (user.Company == null)
                {
                    user.Company = new() { Name = "" };
                }
            }

            return Json(new { data = usersList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            ApplicationUser UserObject = _db.ApplicationUsers.FirstOrDefault(data => data.Id == id);
            if (UserObject is null)
                return Json(new { success = false, message = "Error While Locking/Unlocking User" });


            if (UserObject.LockoutEnd is not null && UserObject.LockoutEnd > DateTime.Now)
                UserObject.LockoutEnd = DateTime.Now;
            else
                UserObject.LockoutEnd = DateTime.Now.AddYears(1000);

            _db.SaveChanges();
            return Json(new { success = true, message = "Operation Successfully" });
        }


        [HttpPost]
        public async Task<IActionResult> Notification([FromBody] string NotifyMessage)
        {
            await _webSocketManager.BroadcastMessageAsync(NotifyMessage);
            return Ok();
        }


        public IActionResult RoleManagement(string Userid)
        {
            var RoleID = _db.UserRoles.FirstOrDefault(data => data.UserId == Userid).RoleId;

            RoleManagementVM RoleManagement = new RoleManagementVM
            {
                ApplicationUser = _db.ApplicationUsers.FirstOrDefault(data => data.Id == Userid),
                RoleList = _db.Roles.Select(data => new SelectListItem
                {
                    Text = data.Name,
                    Value = data.Name,
                }),
                CompanyList = _db.Companies.Select(data => new SelectListItem
                {
                    Text = data.Name,
                    Value = data.ID.ToString(),
                }),
            };

            RoleManagement.ApplicationUser.Role = _db.Roles.FirstOrDefault(data => data.Id == RoleID).Name;

            return View(RoleManagement);
        }


        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM _roleManagementVM)
        {
            var RoleID = _db.UserRoles.FirstOrDefault(data => data.UserId == _roleManagementVM.ApplicationUser.Id).RoleId;
            var OldRole = _db.Roles.FirstOrDefault(data => data.Id == RoleID).Name;

            if (_roleManagementVM.ApplicationUser.Role == OldRole)
                return RedirectToAction(nameof(RoleManagement));


            ApplicationUser _applicationUser = _db.ApplicationUsers.FirstOrDefault(data => data.Id == _roleManagementVM.ApplicationUser.Id);

            if (_roleManagementVM.ApplicationUser.Role == SD.Role_Company)
                _applicationUser.CompanyID = _roleManagementVM.ApplicationUser.CompanyID;

            if (OldRole == SD.Role_Company)
                _applicationUser.CompanyID = null;

            _db.SaveChanges();

            _userManager.RemoveFromRoleAsync(_applicationUser, OldRole).GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(_applicationUser, _roleManagementVM.ApplicationUser.Role).GetAwaiter().GetResult();

            TempData["success"] = "Role Updated";


            return RedirectToAction(nameof(Index));
        }
    }
}
