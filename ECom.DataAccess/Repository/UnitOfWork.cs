using E_Commerce.DataAccess.Data;
using ECom.DataAccess.Repository.IRepository;
using ECom.Utility.Interface;
using ECom.Utility.Services;
using Microsoft.EntityFrameworkCore;

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

        private readonly ICacheService _cacheService;

        public UnitOfWork(ApplicationDbContext db, ICacheService cacheService)
        {
            _db = db;
            _cacheService = cacheService;
            ApplicationUsers = new ApplicationUserRepository(_db, _cacheService);
            Category = new CategoryRepository(_db, _cacheService);
            Product = new ProductRepository(_db, _cacheService);
            Company = new CompanyRepository(_db, _cacheService);
            ShoppingCarts = new ShoppingCartRepository(_db, _cacheService);
            OrderDetails = new OrderDetailRepository(_db, _cacheService);
            OrderHeaders = new OrderHeaderRepository(_db, _cacheService);
            OrderPayments = new OrderPaymentRepository(_db, _cacheService);
        }

        public void Save(bool resetCache = false)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    if (resetCache)
                        ResetCacheKeys().GetAwaiter().GetResult();

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


        public async Task SaveAsync(bool resetCache = false)
        {
            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    if (resetCache)
                        await ResetCacheKeys();

                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }


        private async Task ResetCacheKeys()
        {
            var changedTables = _db.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
            .Select(e => e.Entity.GetType().Name)
            .Distinct()
            .ToList();

            foreach (var TablesKeys in changedTables)
            {
                _ = await _cacheService.RemoveAsync(TablesKeys);
            }
        }


    }
}
