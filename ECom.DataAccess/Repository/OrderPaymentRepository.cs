using E_Commerce.DataAccess.Data;
using ECom.DataAccess.Repository.IRepository;
using ECom.Models.ViewModels;

namespace ECom.DataAccess.Repository
{
    public class OrderPaymentRepository : Repository<OrderPayment>, IOrderPaymentRepository
    {
        private ApplicationDbContext _db;
        public OrderPaymentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderPayment OrderPaymentObject)
        {
            _db.OrderPayments.Update(OrderPaymentObject);
        }
    }
}
