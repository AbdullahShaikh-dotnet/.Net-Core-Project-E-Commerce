using System.Security.Claims;
using ECom.Utility;
using Microsoft.AspNetCore.Http;

public interface IUserService
{
    string GetUserId();
    int? GetCartCount();
    void SetCartCount(int CartCount);
    void ClearCart();
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
        _httpContextAccessor.HttpContext.Session.SetInt32(SD.ShoppingCartSessionKey, CartCount);
    }

    public void ClearCart()
    {
        _httpContextAccessor.HttpContext.Session.SetInt32(SD.ShoppingCartSessionKey, 0);
    }
}
