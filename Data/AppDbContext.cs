using DatingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data;

public class AppDbContext :DbContext
{
    
    public DbSet<Account> Accounts { get; set; } 
    public DbSet<Favourite> Favourites { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      
        modelBuilder.Entity<Account>()
            .HasMany(u => u.Favourites)
            .WithOne(f => f.Account)
            .HasForeignKey(f => f.AccountId)
            .OnDelete(DeleteBehavior.NoAction); 

        modelBuilder.Entity<Favourite>()
            .HasOne(f => f.FavoriteAccount)
            .WithMany()
            .HasForeignKey(f => f.FavoriteAccountId)
            .OnDelete(DeleteBehavior.NoAction);
        
        base.OnModelCreating(modelBuilder);
    }
}