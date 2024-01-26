using System.ComponentModel.DataAnnotations;

namespace DatingApp.Models;

public class Favourite
{
    [Key]
    public int FavoriteId { get; set; }

    
    public int AccountId { get; set; }
    public Account Account { get; set; }

   
    public int FavoriteAccountId { get; set; }
    public Account FavoriteAccount { get; set; }

   
}