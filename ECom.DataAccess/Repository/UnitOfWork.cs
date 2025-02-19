using E_Commerce.DataAccess.Data;
using ECom.DataAccess.Repository.IRepository;

namespace ECom.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IApplicationUserRepository ApplicationUsers { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IShoppingCart ShoppingCarts { get; private set; }
        public IOrderHeaderRepository OrderHeaders { get; private set; }
        public IOrderDetailRepository OrderDetails { get; private set; }
        public IOrderPaymentRepository OrderPayments { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            ApplicationUsers = new ApplicationUserRepository(_db);
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            Company = new CompanyRepository(_db);
            ShoppingCarts = new ShoppingCartRepository(_db);
            OrderDetails = new OrderDetailRepository(_db);
            OrderHeaders = new OrderHeaderRepository(_db);
            OrderPayments = new OrderPaymentRepository(_db);
        }

        public void Save()
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
