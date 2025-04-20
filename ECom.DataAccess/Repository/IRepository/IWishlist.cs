using ECom.Models;

namespace ECom.DataAccess.Repository.IRepository
{
    public interface IWishlist : IRepository<Wishlist>
    {
        void Update(Wishlist WishlistObj);
    }
}
