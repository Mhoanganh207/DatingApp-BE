namespace DatingApp.Models;

public class InforDto
{
    public string Firstname { get; set; }

    public string Surname { get; set; }
    
    public string Address { get; set; }
    public ICollection<Hobby> Hobbies { get; set; } = new List<Hobby>();
    public string Introduction { get; set; }
}