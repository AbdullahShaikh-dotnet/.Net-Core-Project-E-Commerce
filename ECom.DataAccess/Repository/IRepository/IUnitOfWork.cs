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
        void Save();
    }
}
