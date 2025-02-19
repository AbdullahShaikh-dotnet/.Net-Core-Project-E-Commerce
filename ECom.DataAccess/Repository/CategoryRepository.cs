using E_Commerce.DataAccess.Data;
using ECom.DataAccess.Repository.IRepository;
using ECom.Models;

namespace ECom.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Category CategoryObject)
        {
            _db.Categories.Update(CategoryObject);
        }
    }
}
