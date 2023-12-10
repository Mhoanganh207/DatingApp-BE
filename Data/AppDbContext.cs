using DatingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data;

public class AppDbContext :DbContext
{
    
    public DbSet<Account> Accounts { get; set; } 

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
    }
}