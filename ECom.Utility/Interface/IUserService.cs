using ECom.Utility;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public interface IUserService
{
    string GetUserId();


    // Cart
    int? GetCartCount();
    void SetCartCount(int CartCount);
    void ClearCart();


    // Wishlist
    int? GetWishlistCount();
    void SetWishlistCount(int WishlistCount);
    void ClearWishlist();

}

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetUserId()
    {
        var userClaims = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
        return userClaims?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public int? GetCartCount()
    {
        var CartCount = _httpContextAccessor.HttpContext.Session.GetInt32(SD.ShoppingCartSessionKey);
        return CartCount;
    }

    public void SetCartCount(int CartCount)
    {
        _httpContextAccessor.HttpContext.Session.SetInt32(SD.ShoppingCartSessionKey, (int)CartCount);
    }

    public void ClearCart()
    {
        _httpContextAccessor.HttpContext.Session.SetInt32(SD.ShoppingCartSessionKey, 0);
    }



    public int? GetWishlistCount()
    {
        var WishlistCount = _httpContextAccessor.HttpContext.Session.GetInt32(SD.WishlistSessionKey);
        return WishlistCount;
    }

    public void SetWishlistCount(int WishlistCount)
    {
        _httpContextAccessor.HttpContext.Session.SetInt32(SD.WishlistSessionKey, (int)WishlistCount);
    }

    public void ClearWishlist()
    {
        _httpContextAccessor.HttpContext.Session.SetInt32(SD.WishlistSessionKey, 0);
    }

}
