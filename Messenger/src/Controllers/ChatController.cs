using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger;

[Authorize]
public class ChatController : Controller
{
    private readonly IMessageService _chatService;

    private readonly IMessagesOperationService _messagesOperationService;

    private readonly IClaimService _claimService;

    private readonly IMetricsService _metricsService;

    public ChatController(
        IMessageService chatService,
        IMessagesOperationService messagesOperationService,
        IClaimService claimService,
        ILogger<ChatController> logger,
        IMetricsService metricsService)
    {
        _chatService = chatService;
        _messagesOperationService = messagesOperationService;
        _claimService = claimService;
        _metricsService = metricsService;
    }

    [HttpPost("/SendMessage")]
    public async Task<IActionResult> SendMessage([FromForm] string recipientId, [FromForm] string content)
    {

        await _chatService.SaveMessageAsync(
            _claimService.GetUserId(),
            recipientId,
            _claimService.GetUserName(),
            content);

        await _messagesOperationService.SendMessageAsync(new MessageDto
        {
            RecipientId = recipientId,
            Content = content,
            SenderName = _claimService.GetUserName()
        });

        await _messagesOperationService.SendMessageAsync(new MessageDto
        {
            RecipientId = _claimService.GetUserId(),
            Content = content,
            SenderName = _claimService.GetUserName()
        });

        _metricsService.MessageSent(
            _claimService.GetUserId(),
            recipientId,
            content);

        return NoContent();
    }

    [HttpDelete("/DeleteMessage")]
    public async Task<IActionResult> DeleteMessage([FromQuery] string messageId, [FromQuery] string interlocutorId)
    {
        await _chatService.DeleteMessageByIdAsync(messageId);
        await _messagesOperationService.DeleteMessageToInterlocutorAsync(interlocutorId, messageId);
        return NoContent();
    }
}