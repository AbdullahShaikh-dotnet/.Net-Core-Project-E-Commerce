using E_Commerce.DataAccess.Data;
using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Utility.Interface;

namespace ECom.DataAccess.Repository
{
    public class WishlistRepository : Repository<Wishlist>, IWishlist
    {
        private ApplicationDbContext _db;
        public WishlistRepository(ApplicationDbContext db, ICacheService cacheService) : base(db, cacheService)
        {
            _db = db;
        }

        public void Update(Wishlist WishlistObj)
        {
            _db.Wishlist.Update(WishlistObj);
        }
    }
}
