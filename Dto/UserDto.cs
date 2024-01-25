namespace DatingApp.Models;

public class UserDto
{
    public int Id { get; set; }
    public string Firstname { get; set; }
    public string Surname { get; set; }
    public string Avatar { get; set; }
    

    public UserDto(Account account)
    {
        this.Id = account.Id;
        this.Firstname = account.Firstname;
        this.Surname = account.Surname;
        this.Avatar = account.Avatar;
    }

}