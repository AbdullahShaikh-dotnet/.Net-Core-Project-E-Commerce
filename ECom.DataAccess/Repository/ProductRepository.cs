using E_Commerce.DataAccess.Data;
using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Utility.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECom.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db, ICacheService cacheService) : base(db, cacheService)
        {
            _db = db;
        }

        public void Update(Product ProductObject)
        {
            _db.Products.Update(ProductObject);

            Product? _product = _db.Products.FirstOrDefault(d => d.Id == ProductObject.Id);
            if (_product == null) return;

            _product.Title = ProductObject.Title;
            _product.Author = ProductObject.Author;
            _product.Category_ID = ProductObject.Category_ID;
            _product.ISBN = ProductObject.ISBN;
            _product.ListPrice = ProductObject.ListPrice;
            _product.Price = ProductObject.Price;
            _product.Price50 = ProductObject.Price50;
            _product.Price100 = ProductObject.Price100;
            _product.Description = ProductObject.Description;
            _product.ProductImages = ProductObject.ProductImages;
        }

        public void UpdateBulk(IEnumerable<Product> productObjectList)
        {
                _db.Products.UpdateRange(productObjectList);
        }
    }
}
