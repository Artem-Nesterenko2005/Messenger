using System.Security.Claims;

namespace Messenger;

public interface IClaimService
{
    string GetUserId();
    string GetUserName();
}

public class ClaimService : IClaimService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClaimService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetUserId()
    {
        return _httpContextAccessor.HttpContext?.User
            .FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User not authenticated");
    }

    public string GetUserName()
    {
        return _httpContextAccessor.HttpContext?.User
            .FindFirstValue(ClaimTypes.Name);
    }
}
