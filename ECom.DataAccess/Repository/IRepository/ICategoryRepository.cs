using ECom.Models;

namespace ECom.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category CategoryObject);
    }
}
