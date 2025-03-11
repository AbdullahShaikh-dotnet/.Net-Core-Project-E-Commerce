using E_Commerce.DataAccess.Data;
using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Utility;
using ECom.Utility.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebSocketManager _webSocketManager;

        public UserController(ApplicationDbContext db,
            IWebSocketManager webSocketManager)
        {
            _db = db;
            _webSocketManager = webSocketManager;
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
    }
}
