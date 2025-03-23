namespace ECom.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IApplicationUserRepository ApplicationUsers { get; }
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        ICompanyRepository Company { get; }
        IShoppingCart ShoppingCarts { get; }
        IOrderDetailRepository OrderDetails { get; }
        IOrderHeaderRepository OrderHeaders { get; }
        IOrderPaymentRepository OrderPayments { get; }
        IProductImageRepository ProductImages { get; }
        void Save(bool resetCache = false);
        Task SaveAsync(bool resetCache = false);
    }
}
