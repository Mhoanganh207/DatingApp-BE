using System.ComponentModel.DataAnnotations;

namespace DatingApp.Models;

public class Account
{
    [Key]
    public int Id { get; set; }
    public string Username { get; set; }
    [MinLength(6)]
    public string Password { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    public string Avatar { get; set; }
    
}