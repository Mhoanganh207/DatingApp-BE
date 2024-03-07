namespace DatingApp.Models;

public class UserDto
{
    public int Id { get; set; }
    public string Firstname { get; set; }
    public string Surname { get; set; }
    public int Age { get; set; }
    public string Avatar { get; set; }
    public string Interest { get; set; }
    public string Introduction { get; set; }
    
    public UserDto(){
        
    }
    

    public UserDto(Account account)
    {
        this.Id = account.Id;
        this.Firstname = account.Firstname;
        this.Surname = account.Surname;
        this.Age = DateTime.Now.Year - account.BirthDate.Year;
        this.Interest = account.Interest;
        this.Introduction = account.Introduction;
        this.Avatar = account.Avatar;
    }

}