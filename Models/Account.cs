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
    public string? Interest { get; set; }
    public string? Introduction { get; set; }
}