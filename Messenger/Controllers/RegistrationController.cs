using Messenger.Services;
using Microsoft.AspNetCore.Mvc;

namespace Messenger;

public class RegistrationController : Controller
{
    private readonly IUserService _userService;

    public RegistrationController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("Registration")]
    public IActionResult Registration()
    {
        return View();
    }

    [HttpPost("TryRegistration")]
    public async Task<IActionResult> RegisterNewUser(RegistrationUserDto dto)
    {
        var validationErrors = await _userService.TryRegistrationNewUser(dto);
        if (validationErrors != null)
        {
            ViewData["ErrorMessages"] = validationErrors;
            return View("Registration", dto);
        }
        return View("SuccessAuthorization");
    }
}
