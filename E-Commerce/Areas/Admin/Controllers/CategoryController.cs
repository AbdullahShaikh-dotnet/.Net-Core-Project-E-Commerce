using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> categories = _UnitOfWork.Category.GetAll().Where(data => !data.IsDeleted).ToList();
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

                _UnitOfWork.Category.Add(obj);
                _UnitOfWork.Save();
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

            List<Category> categories = _UnitOfWork.Category.GetAll().ToList();
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

            Category? category = _UnitOfWork.Category.Get(cat => Id == cat.Id);
            if (category == null) return NotFound();

            return View(category);
        }


        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (obj.Id == 0) return NotFound();

            if (!ModelState.IsValid) return View();

            _UnitOfWork.Category.Update(obj);
            _UnitOfWork.Save();
            TempData["success"] = "Category Updated Successfully";
            return RedirectToAction("Index");

        }


        //public IActionResult Delete(int? Id)
        //{
        //    if (Id == null || Id == 0) return NotFound();

        //    Category? category = _UnitOfWork.Category.Get(cat => Id == cat.Id);
        //    if (category == null) return NotFound();

        //    return View(category);
        //}


        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(Category obj)
        {
            Category? Db_Category = _UnitOfWork.Category.Get(cat => obj.Id == cat.Id);

            if (Db_Category == null) return NotFound();

            // _db.Categories.Remove(Db_Category);

            Db_Category.IsDeleted = true;
            Db_Category.DeletedAt = DateTime.Now;
            _UnitOfWork.Save();
            TempData["success"] = "Category Deleted Successfully";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult GetAll()
        {
            List<Category> CompaniesList = _UnitOfWork.Category.GetAll()
                .Where(data => !data.IsDeleted).ToList();
            return Json(new { data = CompaniesList });
        }


        public IActionResult Delete(int? id)
        {
            var CategoryToBeDeleted = _UnitOfWork.Category.Get(d => d.Id == id);
            if (CategoryToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error While Deletig" });
            }

            CategoryToBeDeleted.IsDeleted = true;
            CategoryToBeDeleted.DeletedAt = DateTime.Now;
            _UnitOfWork.Category.Update(CategoryToBeDeleted);
            _UnitOfWork.Save();

            return Json(new { success = true, message = "Deleted Successfully" });
        }


        public IActionResult Upsert(int? id)
        {
            if (id == 0 || id is null)
                return View(new Category());

            var CategoryObj = _UnitOfWork.Category.Get(data => data.Id == id);
            return View(CategoryObj);
        }
    }
}
