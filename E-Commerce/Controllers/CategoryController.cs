using E_Commerce.Data;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;

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
            List<Category> categories = _db.Categories.ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (int.TryParse(obj.Name, out int name))
                ModelState.AddModelError("name", "Name Should not be Number");

            List<Category> categories = _db.Categories.ToList();
            var isAlreadyExists = categories.Exists(d => d.Name == obj.Name);

            if (isAlreadyExists)
                ModelState.AddModelError("name", $"{obj.Name} is Already Exists");


            if (!ModelState.IsValid) return View();

            _db.Categories.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }


        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id == 0) return NotFound();

            Category? category = _db.Categories.Find(Id);
            if(category == null) return NotFound();


            return View(category);
        }

    }
}
