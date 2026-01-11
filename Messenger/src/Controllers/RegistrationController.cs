using Messenger.Services;
using Microsoft.AspNetCore.Mvc;

namespace Messenger;

public class RegistrationController : Controller
{
    private readonly IUserAuthenticationService _userService;

    private readonly IUserValidationService _userValidationService;

    public RegistrationController(IUserAuthenticationService userService, IUserValidationService validationService)
    {
        _userService = userService;
        _userValidationService = validationService;
    }

    [HttpGet("Registration")]
    public IActionResult Registration()
    {
        return View();
    }

    [HttpPost("TryRegistration")]
    public async Task<IActionResult> RegisterNewUser([FromForm] RegistrationUserDto dto)
    {
        var validationErrors = await _userValidationService.ValidateRegistrationAsync(dto);
        if (validationErrors != null)
        {
            return View("Registration", validationErrors);
        }
        await _userService.RegistrationNewUser(dto);
        return View("SuccessAuthorization");
    }
}
