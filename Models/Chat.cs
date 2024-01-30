using System.ComponentModel.DataAnnotations;

namespace DatingApp.Models;

public class Chat
{
    [Key]
    public int Id { get; set; }
    public ICollection<Account> Accounts { get; set;} = new List<Account>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<AccountChat>? AccountChats { get; set; } = new List<AccountChat>();
}