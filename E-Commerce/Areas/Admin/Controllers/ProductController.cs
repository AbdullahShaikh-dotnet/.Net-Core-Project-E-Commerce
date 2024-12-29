using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            List<Product> Products = _UnitOfWork.Product.GetAll()
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

            if (id == 0 || id == null)
                return View(productViewModel);

            productViewModel.product = _UnitOfWork.Product.Get(data => data.Id == id);
            return View(productViewModel);
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM _productVM, IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string Filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string ProductImagePath = Path.Combine(wwwRootPath, @"Images\Products");
                    string CompleteFilePath = Path.Combine(ProductImagePath, Filename);

                    if (!Directory.Exists(ProductImagePath))
                        Directory.CreateDirectory(ProductImagePath);

                    if (!string.IsNullOrEmpty(_productVM.product.ImageURL))
                    {
                        string OldFileName = Path.Combine(wwwRootPath, _productVM.product.ImageURL.TrimStart('\\'));

                        if (System.IO.File.Exists(OldFileName))
                            System.IO.File.Delete(OldFileName);
                    }

                    using (var fileStream = new FileStream(CompleteFilePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    _productVM.product.ImageURL = @$"\Images\Products\{Filename}";
                }

                if (_productVM.product.Id == 0)
                    _UnitOfWork.Product.Add(_productVM.product);
                else
                    _UnitOfWork.Product.Update(_productVM.product);

                _UnitOfWork.Save();
                TempData["success"] = "Product Created Successfully";
                return RedirectToAction("Index");
            }
            else
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
