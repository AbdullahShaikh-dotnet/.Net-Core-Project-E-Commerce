using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECom.Models;
using ECom.Models.ViewModels;

namespace ECom.DataAccess.Repository.IRepository
{
    public interface IOrderPaymentRepository : IRepository<OrderPayment>
    {
        void Update(OrderPayment OrderPaymentObject);
    }
}
