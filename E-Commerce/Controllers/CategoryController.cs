using System.Security.Cryptography;
using E_Commerce.Data;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace E_Commerce.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> categories = _db.Categories.Where(data => !data.IsDeleted).ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (Validation(obj))
            {
                if (!ModelState.IsValid) return View();

                _db.Categories.Add(obj);
                _db.SaveChanges();
                TempData["success"] = "Category Created Successfully";
                return RedirectToAction("Index");
            }

            return View(obj);
        }


        private bool Validation(Category obj)
        {
            if (int.TryParse(obj.Name, out int name))
            {
                ModelState.AddModelError("name", "Name Should not be Number");
                return false;
            }

            List<Category> categories = _db.Categories.ToList();
            var isAlreadyExists = categories.Exists(d => d.Name == obj.Name && !d.IsDeleted);

            if (isAlreadyExists)
            {
                ModelState.AddModelError("name", $"{obj.Name} is Already Exists");
                return false;
            }

            return true;
        }

        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id == 0) return NotFound();

            Category? category = _db.Categories.Find(Id);
            if (category == null) return NotFound();

            return View(category);
        }


        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (obj.Id == 0) return NotFound();

            if (!ModelState.IsValid) return View();

            _db.Categories.Update(obj);
            _db.SaveChanges();
            TempData["success"] = "Category Updated Successfully";
            return RedirectToAction("Index");

        }


        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0) return NotFound();

            Category? category = _db.Categories.Find(Id);
            if (category == null) return NotFound();

            return View(category);
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(Category obj)
        {
            Category? Db_Category = _db.Categories.Find(obj.Id);

            if (Db_Category == null) return NotFound();

            // _db.Categories.Remove(Db_Category);

            Db_Category.IsDeleted = true;
            Db_Category.DeletedAt = DateTime.Now;
            _db.SaveChanges();
            TempData["success"] = "Category Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
