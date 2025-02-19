using ECom.Models;

namespace ECom.DataAccess.Repository.IRepository
{
    public interface IShoppingCart : IRepository<ShoppingCart>
    {
        void Update(ShoppingCart ShoppingCartObj);
    }
}
