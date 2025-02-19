using E_Commerce.DataAccess.Data;
using ECom.DataAccess.Repository.IRepository;
using ECom.Models;

namespace ECom.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Company CompanyObject)
        {
            _db.Companies.Update(CompanyObject);
        }
    }
}
