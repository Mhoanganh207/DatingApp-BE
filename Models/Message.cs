using System.ComponentModel.DataAnnotations;

namespace DatingApp.Models;

public class Message
{
    [Key]
    public long Id { get; set; }
    public int SentId { get; set; }
    public Account Sent { get; set; }
    public int ChatId { get; set; }
    public Chat Chat { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
}