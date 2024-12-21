using System.Security.Cryptography;
using E_Commerce.DataAccess.Data;
using ECom.DataAccess.Repository;
using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace E_Commerce.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(ICategoryRepository categoryRepo)
        {
            _categoryRepository = categoryRepo;
        }
        public IActionResult Index()
        {
            List<Category> categories = _categoryRepository.GetAll().Where(data => !data.IsDeleted).ToList();
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

                _categoryRepository.Add(obj);
                _categoryRepository.Save();
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

            List<Category> categories = _categoryRepository.GetAll().ToList();
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

            Category? category = _categoryRepository.Get(cat => Id == cat.Id);
            if (category == null) return NotFound();

            return View(category);
        }


        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (obj.Id == 0) return NotFound();

            if (!ModelState.IsValid) return View();

            _categoryRepository.Update(obj);
            _categoryRepository.Save();
            TempData["success"] = "Category Updated Successfully";
            return RedirectToAction("Index");

        }


        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0) return NotFound();

            Category? category = _categoryRepository.Get(cat => Id == cat.Id);
            if (category == null) return NotFound();

            return View(category);
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(Category obj)
        {
            Category? Db_Category = _categoryRepository.Get(cat => obj.Id == cat.Id);

            if (Db_Category == null) return NotFound();

            // _db.Categories.Remove(Db_Category);

            Db_Category.IsDeleted = true;
            Db_Category.DeletedAt = DateTime.Now;
            _categoryRepository.Save();
            TempData["success"] = "Category Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
