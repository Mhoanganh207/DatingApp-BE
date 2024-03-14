using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DatingApp.Models;


public class Hobby{

    [Key]
    public int Id{ get; set;}
    public String Name { get; set;}

    [JsonIgnore]
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}