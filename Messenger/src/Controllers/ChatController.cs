using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger;

[Authorize]
public class ChatController : Controller
{
    private readonly IChatService _chatService;

    private readonly IMessagingService _messagingService;

    private readonly IClaimService _claimService;

    public ChatController(
        IChatService chatService,
        IMessagingService messagingService,
        IClaimService claimService)
    {
        _chatService = chatService;
        _messagingService = messagingService;
        _claimService = claimService;
    }

    [HttpPost("/SendMessage")]
    public async Task<IActionResult> SendMessage([FromForm] string recipientId, [FromForm] string content)
    {

        await _chatService.SaveMessageAsync(
            _claimService.GetUserId(),
            recipientId,
            _claimService.GetUserName(),
            content);

        await _messagingService.SendMessageAsync(new MessageDto
        {
            RecipientId = recipientId,
            Content = content,
            SenderName = _claimService.GetUserName()
        });

        await _messagingService.SendMessageAsync(new MessageDto
        {
            RecipientId = _claimService.GetUserId(),
            Content = content,
            SenderName = _claimService.GetUserName()
        });
        return NoContent();
    }
}