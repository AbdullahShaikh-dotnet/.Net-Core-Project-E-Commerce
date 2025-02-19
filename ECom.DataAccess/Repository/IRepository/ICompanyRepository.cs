using ECom.Models;

namespace ECom.DataAccess.Repository.IRepository
{
    public interface ICompanyRepository : IRepository<Company>
    {
        void Update(Company CompanyObject);
    }
}
