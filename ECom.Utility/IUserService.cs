using System.Security.Claims;
using Microsoft.AspNetCore.Http;

public interface IUserService
{
    string GetUserId();
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
}
