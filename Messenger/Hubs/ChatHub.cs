using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace Messenger;

public class ChatHub : Hub
{
    public async Task Message(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}