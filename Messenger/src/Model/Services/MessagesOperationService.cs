using Microsoft.AspNetCore.SignalR;

namespace Messenger;

public interface IMessagesOperationService
{
    public Task SendMessageAsync(MessageDto message);

    public Task DeleteMessageToInterlocutorAsync(string interlocutorId, string messageId);
}

public class MessagesOperationService : IMessagesOperationService
{
    private readonly IHubContext<ChatHub> _hub;

    public MessagesOperationService(IHubContext<ChatHub> hub)
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

    public async Task DeleteMessageToInterlocutorAsync(string interlocutorId, string messageId)
    {
        await _hub.Clients.User(interlocutorId).SendAsync("DeleteMessage", messageId);
    }
}
