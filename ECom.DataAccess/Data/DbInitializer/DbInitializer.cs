using E_Commerce.DataAccess.Data;
using ECom.Models;
using ECom.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECom.DataAccess.Data.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        public readonly UserManager<ApplicationUser> _userManager;
        public readonly RoleManager<IdentityRole> _roleManager;
        public readonly ApplicationDbContext _db;
        public DbInitializer(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }
        public void Initialize()
        {
            // Migrate if not Applied
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) { }



            // Create Role if Not Exists
            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();


                // If Role is not available create Admin User
                string Email = "Admin@gmail.com";
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = Email,
                    Email = Email,
                    Name = "Admin",
                    PhoneNumber = "9619068814",
                    StreetAddress = "N/A",
                    State = "N/A",
                    PostalCode = "400060",
                    City = "Mumbai"
                }, "AdminA380!").GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(data => data.Email == Email);
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }

            return;

        }
    }
}
