﻿using ECom.Models;

namespace ECom.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader OrderHeaderObject);

        void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
        void UpdatePaymentGatewayID(int OrderID);
    }
}
