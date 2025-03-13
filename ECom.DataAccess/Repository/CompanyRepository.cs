using E_Commerce.DataAccess.Data;
using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Utility.Interface;

namespace ECom.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext db, ICacheService cacheService) : base(db, cacheService)
        {
            _db = db;
        }

        public void Update(Company CompanyObject)
        {
            _db.Companies.Update(CompanyObject);
        }
    }
}
