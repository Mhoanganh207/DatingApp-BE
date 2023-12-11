using DatingApp.Data;
using DatingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Repository;

public class AccountRepository : IAccountRepository
{
    private readonly AppDbContext _db ;

    public AccountRepository(AppDbContext db)
    {
        this._db = db;
    }


    public async Task<Account> Register(Account account)
    {
        var pass = BCrypt.Net.BCrypt.HashPassword(account.Password);
        account.Password = pass;
        await _db.Accounts.AddAsync(account);
        await _db.SaveChangesAsync();

        return account;
    }

    public async Task<Account> GetById(int id)
    {
        var account = await _db.Accounts.FindAsync(id);

        if (account == null)
        {
            throw new NullReferenceException("No account");
        }
       
        return account;
    }

    public async Task<bool> LogIn(string username, string password)
    {
        var account = await _db.Accounts.FirstOrDefaultAsync(acc => acc.Username == username);
        
        if (account == null)
            return false;
        
        if (!BCrypt.Net.BCrypt.Verify(password, account.Password))
            return false;
        
        return true;
    }
}