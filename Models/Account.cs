using System.ComponentModel.DataAnnotations;


namespace DatingApp.Models;

public class Account
{
    [Key]
    public int Id { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    [MinLength(6)]
    public string Password { get; set; }
    public string Firstname { get; set; }
    public string Surname { get; set; }
    public DateTime BirthDate { get; set; }
    // true is male, false is female
    public bool Gender { get; set; }
    public string? Avatar { get; set; }
    public string? Introduction { get; set; }
    
    public ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();
    public ICollection<Chat> Chats { get; set; } = new List<Chat>();
    public ICollection<AccountChat> AccountChats { get; set; } = new List<AccountChat>();
    public ICollection<Hobby> Hobbies { get; set; } = new List<Hobby>();
}