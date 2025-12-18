using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Messenger;

public interface IAuthorizationService
{
    public Task<IResult> AddCookies(string name, string? returnUrl, HttpContext context);
}

public class AuthorizationService : IAuthorizationService
{
    public async Task<IResult> AddCookies(string name, string? returnUrl, HttpContext context)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, name) };
        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        return Results.Redirect(returnUrl ?? "/");
    }
}
