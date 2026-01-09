using Microsoft.AspNetCore.SignalR;

namespace Messenger;

public interface ISendMessagesService
{
    public Task SendMessageAsync(MessageDto message);
}

public class SendMessagesService : ISendMessagesService
{
    private readonly IHubContext<ChatHub> _hub;

    public SendMessagesService(IHubContext<ChatHub> hub)
    {
        _hub = hub;
    }

    public async Task SendMessageAsync(MessageDto message)
    {
        await _hub.Clients.User(message.RecipientId).SendAsync("Receive", new
        {
            senderName = message.SenderName,
            content = message.Content,
            timestamp = DateTime.Now
        });
    }
}
