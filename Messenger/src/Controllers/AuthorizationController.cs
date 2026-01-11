using Messenger.Services;
using Microsoft.AspNetCore.Mvc;

namespace Messenger;

public class AuthorizationController : Controller
{
    private readonly IUserAuthenticationService _userService;

    private readonly IUserValidationService _userValidationService;

    public AuthorizationController(IUserAuthenticationService userService, IUserValidationService userValidationService)
    {
        _userService = userService;
        _userValidationService = userValidationService;
    }

    [HttpGet("Authorization")]
    public IActionResult Authorization()
    {
        return View();
    }

    [HttpPost("TryAuthorization")]
    public async Task<IActionResult> TryAuthorization([FromForm] AuthorizationUserDto dto)
    {
        var validationErrors = await _userValidationService.ValidateAuthorizationAsync(dto);
        var authorizationErrors = await _userService.Authorization(dto);
        if (validationErrors != null || authorizationErrors != null)
        {
            return View("Authorization", validationErrors ?? authorizationErrors);
        }
        return RedirectToAction("MainPage", "Main");
    }
}
