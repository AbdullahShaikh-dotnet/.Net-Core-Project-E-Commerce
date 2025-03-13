using E_Commerce.DataAccess.Data;
using ECom.DataAccess.Repository.IRepository;
using ECom.Models.ViewModels;
using ECom.Utility.Interface;

namespace ECom.DataAccess.Repository
{
    public class OrderPaymentRepository : Repository<OrderPayment>, IOrderPaymentRepository
    {
        private ApplicationDbContext _db;
        public OrderPaymentRepository(ApplicationDbContext db, ICacheService cacheService) : base(db, cacheService)
        {
            _db = db;
        }

        public void Update(OrderPayment OrderPaymentObject)
        {
            _db.OrderPayments.Update(OrderPaymentObject);
        }
    }
}
