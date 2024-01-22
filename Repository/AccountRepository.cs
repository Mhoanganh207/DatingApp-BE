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

    public async Task<bool> LogIn(string email, string password)
    {
        var account = await _db.Accounts.FirstOrDefaultAsync(acc => acc.Email == email);
        
        if (account == null)
            return false;
        
        if (!BCrypt.Net.BCrypt.Verify(password, account.Password))
            return false;
        
        return true;
    }

    public async Task<Account> UpdateAvatar(Account account)
    {
        try
        {
            var acc = await this.GetById(account.Id);
                acc.Avatar = account.Avatar;
                await this._db.SaveChangesAsync();
                return acc;
        }
        catch
        {
            throw new Exception("Error occured");
        }
    }

    public async Task<Account> GetAccountByEmail(string email)
    {
        var account = await _db.Accounts.FirstOrDefaultAsync(account1 => account1.Email == email);
        if (account != null)
        {
            return account;
        }
        else
        {
            throw new Exception("No account");
        }
    }
}