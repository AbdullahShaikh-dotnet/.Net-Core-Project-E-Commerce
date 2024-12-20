using E_Commerce_Temp.Data;
using E_Commerce_Temp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Temp.Pages.Categories
{
    [BindProperties]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public Category Category { get; set; }
        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet(int Id)
        {
            Category = _db.Categories.Find(Id);
        }

        public IActionResult OnPost() {
            var obj = _db.Categories.Find(Category.Id);
            obj.IsDeleted = true;
            obj.DeletedAt = DateTime.Now;
            _db.SaveChanges();
            TempData["success"] = "Category Deleted Successfully";
            return RedirectToPage("Index");
        }
    }
}
