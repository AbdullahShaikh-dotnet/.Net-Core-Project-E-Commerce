using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Models.ViewModels;
using ECom.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;

        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment iWebHostEnvironment)
        {
            _UnitOfWork = unitOfWork;
            _webHostEnvironment = iWebHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> Products = _UnitOfWork.Product.GetAll(includePropertiesList: "Category")
                .Where(data => !data.IsDeleted).ToList();
            return View(Products);
        }

        public IActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> Category = _UnitOfWork.Category
                .GetAll()
                .Where(a => !a.IsDeleted)
                .Select(d => new SelectListItem
                {
                    Text = d.Name,
                    Value = d.Id.ToString(),
                });

            ProductVM productViewModel = new ProductVM()
            {
                categoryList = Category,
                product = new Product()
            };

            if (id == 0 || id is null)
                return View(productViewModel);

            productViewModel.product = _UnitOfWork.Product.Get(data => data.Id == id, includePropertiesList: "ProductImages");
            return View(productViewModel);
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFileCollection files)
        {
            if (!ModelState.IsValid)
            {
                productVM.categoryList = _UnitOfWork.Category
                    .GetAll().Where(a => !a.IsDeleted)
                    .Select(d => new SelectListItem
                    {
                        Text = d.Name,
                        Value = d.Id.ToString()
                    }).ToList();  // Ensuring immediate execution

                return View(productVM);
            }

            if (productVM.product.Id == 0)
                _UnitOfWork.Product.Add(productVM.product);
            else
                _UnitOfWork.Product.Update(productVM.product);

            _UnitOfWork.Save();

            if (files is not null && files.Count > 0)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string folderPath = Path.Combine("Media", "Products", $"Product-{productVM.product.Id}");
                string productImagePath = Path.Combine(wwwRootPath, folderPath);

                if (!Directory.Exists(productImagePath))
                    Directory.CreateDirectory(productImagePath);

                foreach (var file in files)
                {
                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    string completeFilePath = Path.Combine(productImagePath, fileName);

                    using (var fileStream = new FileStream(completeFilePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    var productImage = new ProductImages
                    {
                        ImageURL = $"/{folderPath}/{fileName}",
                        ProductID = productVM.product.Id
                    };

                    productVM.product.ProductImages ??= new List<ProductImages>();
                    productVM.product.ProductImages.Add(productImage);
                }

                _UnitOfWork.Product.Update(productVM.product);
                _UnitOfWork.Save();
            }

            TempData["success"] = "Product Created Successfully";
            return RedirectToAction("Index");
        }



        //public IActionResult Delete(int? Id)
        //{
        //    if (Id == null || Id == 0) return NotFound();

        //    Product? product = _UnitOfWork.Product.Get(cat => Id == cat.Id);
        //    if (product == null) return NotFound();

        //    return View(product);
        //}

        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePost(Product obj)
        //{
        //    Product? Db_Product = _UnitOfWork.Product.Get(cat => obj.Id == cat.Id);

        //    if (Db_Product == null) return NotFound();


        //    Db_Product.IsDeleted = true;
        //    Db_Product.DeletedAt = DateTime.Now;
        //    _UnitOfWork.Save();
        //    TempData["success"] = "Product Deleted Successfully";
        //    return RedirectToAction("Index");
        //}

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> ProductsList = _UnitOfWork.Product.GetAll(includePropertiesList: "Category")
                .Where(data => !data.IsDeleted).ToList();
            return Json(new { data = ProductsList });
        }


        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _UnitOfWork.Product.Get(d => d.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error While Deletig" });
            }

            //string wwwRootPath = _webHostEnvironment.WebRootPath;
            //if (!string.IsNullOrEmpty(productToBeDeleted.ImageURL))
            //{
            //    string OldFileName = Path.Combine(wwwRootPath, productToBeDeleted.ImageURL.TrimStart('\\'));

            //    if (System.IO.File.Exists(OldFileName))
            //        System.IO.File.Delete(OldFileName);
            //}

            productToBeDeleted.IsDeleted = true;
            productToBeDeleted.DeletedAt = DateTime.Now;
            _UnitOfWork.Product.Update(productToBeDeleted);
            _UnitOfWork.Save();

            return Json(new { success = true, message = "Deleted Successfully" });
        }
    }
}
