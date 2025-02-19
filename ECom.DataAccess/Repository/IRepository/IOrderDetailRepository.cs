using ECom.Models;

namespace ECom.DataAccess.Repository.IRepository
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail OrderDetailObject);
    }
}
