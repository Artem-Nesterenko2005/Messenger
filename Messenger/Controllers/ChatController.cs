using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Messenger;

[Authorize]
public class ChatController : Controller
{
    private readonly IChatService _chatService;

    private readonly IHubContext<ChatHub> _hub;

    public ChatController(IChatService chatService, IHubContext<ChatHub> hub)
    {
        _chatService = chatService;
        _hub = hub;
    }

    [HttpPost("/SendMessage")]
    public async Task<IActionResult> SendMessage([FromForm] string recipientId, [FromForm] string content)
    {

        await _chatService.SaveMessageAsync(
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value!,
            recipientId,
            User.FindFirst(ClaimTypes.Name)?.Value!,
            content);

        await _hub.Clients.User(recipientId).SendAsync("Receive", new
        {
            senderName = User.FindFirst(ClaimTypes.Name)?.Value!,
            content = content,
            timestamp = DateTime.Now
        });

        await _hub.Clients.User(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!).SendAsync("Receive", new
        {
            senderName = User.FindFirst(ClaimTypes.Name)?.Value!,
            content = content,
            timestamp = DateTime.Now
        });
        return NoContent();
    }
}