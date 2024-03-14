namespace DatingApp.Models;

public class InforDto
{
    public ICollection<Hobby> Hobbies { get; set; } = new List<Hobby>();
    public string Introduction { get; set; }
}