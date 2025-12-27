using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Messenger;

public class ChatHub : Hub
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ChatHub(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinGroup(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, userId);
    }

    public async Task SendToUser(string userId, string message)
    {
        await Clients.User(userId).SendAsync("Receive", message);
    }
}