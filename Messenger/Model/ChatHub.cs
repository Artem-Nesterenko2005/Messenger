using Messenger;
using Microsoft.AspNetCore.SignalR;

namespace Messenger;

public class ChatHub : Hub
{
    private readonly IMessageRepository _messageRepository;

    public ChatHub(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task SendMessage(string user, string message)
    {
        var msg = new Message
        {
            Id = Guid.NewGuid().ToString(),
            SenderId = Guid.NewGuid().ToString(), //Пока рандомный
            SenderUsername = user,
            Content = message,
            Timestamp = DateTime.UtcNow
        };

        await _messageRepository.AddAsync(msg);
        await Clients.All.SendAsync("ReceiveMessage", user, message, msg.Timestamp);
    }

    public async Task JoinRoom(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        await Clients.Group(roomName).SendAsync("UserJoined", Context.UserIdentifier);
    }

    public async Task LeaveRoom(string roomName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        await Clients.Group(roomName).SendAsync("UserLeft", Context.UserIdentifier);
    }
}
