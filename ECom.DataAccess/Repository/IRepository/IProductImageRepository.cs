using ECom.Models;

namespace ECom.DataAccess.Repository.IRepository
{
    public interface IProductImageRepository : IRepository<ProductImages>
    {
        void Update(ProductImages ProductImagesObject);
    }
}
