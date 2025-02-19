using ECom.Models;

namespace ECom.DataAccess.Repository.IRepository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        void Update(ApplicationUser ApplicationUserObject);
    }
}
