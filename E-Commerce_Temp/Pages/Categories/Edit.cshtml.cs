using E_Commerce_Temp.Data;
using E_Commerce_Temp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Temp.Pages.Categories
{
    [BindProperties]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public Category Category { get; set; }
        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult OnGet(int Id)
        {
            if (Id == 0) return NotFound();
            Category = _db.Categories.Find(Id);

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return NotFound();

            _db.Categories.Update(Category);
            _db.SaveChanges();
            TempData["success"] = "Category Edited Successfully";
            return RedirectToPage("Index");
        }
    }
}
