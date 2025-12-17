using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Messenger;

public record User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required string Id { get; set; }

    [MaxLength(50)]
    public required string Password { get; set; }

    [MaxLength(50)]
    public required string Username { get; set; }

    [MaxLength(100)]
    [EmailAddress]
    public required string Email { get; set; }
}
