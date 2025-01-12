using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECom.Models;

namespace ECom.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader OrderHeaderObject);

        void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);

        void UpdatePaymentGatewayID(int id, string? sessionID = null, string? PaymentIntendID = null);
    }
}
