using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger;

[Authorize]
public class MainController : Controller
{
    private readonly IMessageService _chatService;

    private readonly IChatsService _chatsService;

    private readonly IClaimService _claimService;

    public MainController(IMessageService chatService, IClaimService claimService, IChatsService chatsService)
    {
        _chatService = chatService;
        _claimService = claimService;
        _chatsService = chatsService;
    }

    [HttpGet("/MainPage")]
    public async Task<IActionResult> MainPage()
    {
        var chats = await _chatsService.GetUserInterlocutorsAsync(_claimService.GetUserId());
        return View(new InterlocutorsViewModel 
        { 
            Interlocutors = chats
        });
    }

    [HttpGet("/OpenChat")]
    public async Task<IActionResult> OpenChat([FromQuery] string interlocutorId)
    {
        var chatHistory = await _chatService.GetChatHistoryAsync(_claimService.GetUserId(), interlocutorId);
        return View(new ChatViewModel
        { 
            InterlocutorId = interlocutorId,
            Messages = chatHistory,
            MyId = _claimService.GetUserId(),
            MyName = _claimService.GetUserName(),
        });
    }

    [HttpGet("/SearchInterlocutor")]
    public IActionResult SearchInterlocutor([FromQuery] string interlocutorSubName)
    {
        var interlocutorsBySubName = _chatsService.SearchUsers(interlocutorSubName);
        return Ok(interlocutorsBySubName);
    }
}
