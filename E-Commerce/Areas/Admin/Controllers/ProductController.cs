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

            productViewModel.product = _UnitOfWork.Product.Get(data => data.Id == id);
            return View(productViewModel);
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM _productVM, List<IFormFile?> files)
        {

            if (!ModelState.IsValid)
            {
                _productVM.categoryList = _UnitOfWork.Category
                    .GetAll().Where(a => !a.IsDeleted)
                    .Select(d => new SelectListItem
                    {
                        Text = d.Name,
                        Value = d.Id.ToString()
                    });
                return View(_productVM);
            }


            if (_productVM.product.Id == 0)
                _UnitOfWork.Product.Add(_productVM.product);
            else
                _UnitOfWork.Product.Update(_productVM.product);

            _UnitOfWork.Save();


            if (files is not null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                files.ForEach(file =>
                {
                    string Filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string FolderPath = $@"Media/Products/Product-{_productVM.product.Id}";
                    string ProductImagePath = Path.Combine(wwwRootPath, FolderPath);
                    string CompleteFilePath = Path.Combine(ProductImagePath, Filename);

                    if (!Directory.Exists(ProductImagePath))
                        Directory.CreateDirectory(ProductImagePath);

                    using (var fileStream = new FileStream(CompleteFilePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    ProductImages productImages = new()
                    {
                        ImageURL = $"\\{FolderPath}\\{Filename}",
                        ProductID = _productVM.product.Id
                    };

                    if (_productVM.product.ProductImages is null)
                        _productVM.product.ProductImages = new List<ProductImages>();

                    _productVM.product.ProductImages.Add(productImages);

                });

                _UnitOfWork.Product.Update(_productVM.product);
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
