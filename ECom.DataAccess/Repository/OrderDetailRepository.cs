using E_Commerce.DataAccess.Data;
using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Utility.Interface;

namespace ECom.DataAccess.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private ApplicationDbContext _db;
        public OrderDetailRepository(ApplicationDbContext db, ICacheService cacheService) : base(db, cacheService)
        {
            _db = db;
        }

        public void Update(OrderDetail OrderDetailObject)
        {
            _db.OrderDetails.Update(OrderDetailObject);
        }
    }
}
