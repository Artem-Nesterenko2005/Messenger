using Microsoft.AspNetCore.SignalR;

namespace Messenger;

public class ChatHub : Hub
{
    private readonly IClaimService _claimService;

    public ChatHub(IClaimService claimService)
    {
        _claimService = claimService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = _claimService.GetUserId();

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = _claimService.GetUserId();

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

    public async Task DeleteMessage(string userId, string messageId)
    {
        await Clients.User(userId).SendAsync("DeleteMessage", messageId);
    }
}