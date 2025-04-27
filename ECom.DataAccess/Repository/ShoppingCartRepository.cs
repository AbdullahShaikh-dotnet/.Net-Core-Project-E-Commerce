using E_Commerce.DataAccess.Data;
using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Utility.Interface;

namespace ECom.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCart
    {
        private ApplicationDbContext _db;
        public ShoppingCartRepository(ApplicationDbContext db, ICacheService cacheService) : base(db, cacheService)
        {
            _db = db;
        }

        public void Update(ShoppingCart ShoppingCartObj)
        {
            _db.ShoppingCarts.Update(ShoppingCartObj);
        }

        public void UpdateBulk(IEnumerable<ShoppingCart> ShoppingCartObjList)
        {
            _db.ShoppingCarts.UpdateRange(ShoppingCartObjList);
        }
    }
}
