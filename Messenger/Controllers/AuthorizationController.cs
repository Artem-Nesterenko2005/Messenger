using Messenger.Services;
using Microsoft.AspNetCore.Mvc;

namespace Messenger;

public class AuthorizationController : Controller
{
    private readonly IUserService _userService;

    public AuthorizationController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("Authorization")]
    public IActionResult Authorization()
    {
        return View();
    }

    [HttpPost("TryAuthorization")]
    public async Task<IActionResult> Authorization(AuthorizationUserDto dto)
    {
        var validationErrors = await _userService.TryAuthorization(dto);
        if (validationErrors != null)
        {
            ViewData["ErrorMessages"] = validationErrors;
            return View("Authorization", dto);
        }
        return NoContent();
    }
}
