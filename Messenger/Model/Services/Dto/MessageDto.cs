using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Messenger;

public class MessageDto
{
    public required string SenderName { get; set; }

    public required string RecipientName { get; set; }

    public required string Content { get; set; }
}
