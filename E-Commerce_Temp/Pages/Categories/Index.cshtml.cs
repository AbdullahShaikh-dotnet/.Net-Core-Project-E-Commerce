using E_Commerce_Temp.Data;
using E_Commerce_Temp.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Temp.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public List<Category> Category = new List<Category>();
        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet()
        {
            Category = _db.Categories.Where(data => !data.IsDeleted).ToList();
        }
    }
}
