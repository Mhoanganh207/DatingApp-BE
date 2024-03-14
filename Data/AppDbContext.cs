using DatingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data;

public class AppDbContext :DbContext
{
    
    public DbSet<Account> Accounts { get; set; } 
    public DbSet<Favourite> Favourites { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<AccountChat> AccountChats { get; set; }

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

        modelBuilder.Entity<Account>()
            .HasMany(c => c.Chats)
            .WithMany(a => a.Accounts)
            .UsingEntity<AccountChat>(
                r => r.HasOne<Chat>().WithMany(a => a.AccountChats),
                l => l.HasOne<Account>().WithMany(c => c.AccountChats)
                );
        
        modelBuilder.Entity<Chat>()
            .HasMany(m => m.Messages)
            .WithOne(c => c.Chat);
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sent)
            .WithMany(s => s.SentMessages)
            .HasForeignKey(m => m.SentId);

        modelBuilder.Entity<Account>()
        .HasMany(a => a.Hobbies)
        .WithMany(h => h.Accounts)
        .UsingEntity(j => j.ToTable("AccountHobbies"))
        ;
        
            
        
        base.OnModelCreating(modelBuilder);
    }
}