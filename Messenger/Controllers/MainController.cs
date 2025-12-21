using Messenger.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Messenger;

[Authorize]
public class MainController : Controller
{
    private readonly IChatService _chatService;
    private readonly IUserRepository _userRepository;

    public MainController(IChatService chatService, IUserRepository userRepository)
    {
        _chatService = chatService;
        _userRepository = userRepository;
    }

    [HttpGet("/MainPage")]
    public async Task<IActionResult> MainPage()
    {
        var chats = await _chatService.GetUserChatsAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        ViewData["chatNames"] = chats.Select(e => e.Users.First(w => w.Username != User.Identity!.Name)).ToList();
        return View();
    }

    [HttpGet("/OpenChat")]
    public async Task<IActionResult> OpenChat(string interlocutorId)
    {
        ViewData["chatMessages"] = await _chatService.GetChatHistoryAsync(User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value!, interlocutorId);
        return View();
    }
}
