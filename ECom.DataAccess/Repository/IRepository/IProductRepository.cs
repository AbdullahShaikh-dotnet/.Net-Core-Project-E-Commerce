using ECom.Models;

namespace ECom.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product ProductObject);
    }
}
