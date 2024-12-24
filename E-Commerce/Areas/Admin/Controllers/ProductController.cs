using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Product> Products = _UnitOfWork.Product.GetAll().Where(data => !data.IsDeleted).ToList();
            return View(Products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product obj)
        {
            if (Validation(obj))
            {
                if (!ModelState.IsValid) return View();

                _UnitOfWork.Product.Add(obj);
                _UnitOfWork.Save();
                TempData["success"] = "Product Created Successfully";
                return RedirectToAction("Index");
            }

            return View(obj);
        }


        private bool Validation(Product obj)
        {
            if (int.TryParse(obj.Title, out int name))
            {
                ModelState.AddModelError("name", "Name Should not be Number");
                return false;
            }

            List<Product> categories = _UnitOfWork.Product.GetAll().ToList();
            var isAlreadyExists = categories.Exists(d => d.Title == obj.Title && !d.IsDeleted);

            if (isAlreadyExists)
            {
                ModelState.AddModelError("name", $"{obj.Title} is Already Exists");
                return false;
            }

            return true;
        }

        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id == 0) return NotFound();

            Product? Product = _UnitOfWork.Product.Get(cat => Id == cat.Id);
            if (Product == null) return NotFound();

            return View(Product);
        }


        [HttpPost]
        public IActionResult Edit(Product obj)
        {
            if (obj.Id == 0) return NotFound();

            if (!ModelState.IsValid) return View();

            _UnitOfWork.Product.Update(obj);
            _UnitOfWork.Save();
            TempData["success"] = "Product Updated Successfully";
            return RedirectToAction("Index");

        }


        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0) return NotFound();

            Product? product = _UnitOfWork.Product.Get(cat => Id == cat.Id);
            if (product == null) return NotFound();

            return View(product);
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(Product obj)
        {
            Product? Db_Product = _UnitOfWork.Product.Get(cat => obj.Id == cat.Id);

            if (Db_Product == null) return NotFound();


            Db_Product.IsDeleted = true;
            Db_Product.DeletedAt = DateTime.Now;
            _UnitOfWork.Save();
            TempData["success"] = "Product Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
