using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.DataAccess.Data;
using ECom.DataAccess.Repository.IRepository;
using ECom.Models;

namespace ECom.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader OrderHeaderObject)
        {
            _db.OrderHeaders.Update(OrderHeaderObject);
        }

        public void UpdatePaymentGatewayID(int OrderID)
        {
            var orderPaymentDb = _db.OrderPayments.FirstOrDefault(x => x.OrderID == OrderID);
            if (orderPaymentDb is null) return;

            if (orderPaymentDb.isPaymentSuccessfull)
            {
                var orderHeaderDB = _db.OrderHeaders.FirstOrDefault(x => x.ID == OrderID);
                if (orderHeaderDB is null) return;

                orderHeaderDB.SessionId = orderPaymentDb.razorpay_order_id;

                if (!string.IsNullOrEmpty(orderPaymentDb.razorpay_payment_id))
                {
                    orderHeaderDB.PaymentIntendID = orderPaymentDb.razorpay_payment_id;
                }

                if (!string.IsNullOrEmpty(orderPaymentDb.razorpay_signature))
                {
                    orderHeaderDB.PaymentSignature = orderPaymentDb.razorpay_signature;
                }

            }
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderHeaderDB = _db.OrderHeaders.FirstOrDefault(x => x.ID == id);
            if (orderHeaderDB is null) return;
            orderHeaderDB.OrderStatus = orderStatus;
            if (paymentStatus is not null)
                orderHeaderDB.PaymentStatus = paymentStatus;
        }
    }
}
