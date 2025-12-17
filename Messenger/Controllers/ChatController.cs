using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger;

[Authorize]
public class ChatController : Controller
{
    private readonly IMessageRepository _messageRepository;

    public ChatController(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    [HttpGet("Chat")]
    public IActionResult Chat()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetMessageHistory()
    {
        var messages = await _messageRepository.GetRecentMessagesAsync(100);
        return Json(messages);
    }
}