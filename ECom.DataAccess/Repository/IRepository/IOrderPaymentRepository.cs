using ECom.Models.ViewModels;

namespace ECom.DataAccess.Repository.IRepository
{
    public interface IOrderPaymentRepository : IRepository<OrderPayment>
    {
        void Update(OrderPayment OrderPaymentObject);
    }
}
