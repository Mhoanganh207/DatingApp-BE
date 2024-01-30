using DatingApp.Data;
using DatingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Repository;

public class AccountRepository
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

    public async Task<Account> LogIn(string email, string password)
    {
        var account = await _db.Accounts.FirstOrDefaultAsync(acc => acc.Email == email);
        
        if (account == null)
        {
            throw new NullReferenceException("No account");
        }
        
        if (!BCrypt.Net.BCrypt.Verify(password, account.Password))
        {
            throw new Exception("Wrong password");
        }
        
        return account;
    }

    public async Task<Account> UpdateAccount(Account account)
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
    public async Task<List<Account>> GetUser(int id,int page)
    {
        var favouriteList = _db.Accounts.Where(account => account.Id == id).Join(_db.Favourites,
            a => a.Id,
            f => f.AccountId,
            (a, f) => new
            {
                Id = f.FavoriteAccountId
            }
        ).ToList();
        return _db.Accounts.Skip((page-1)*10).Take(10).ToList().Where(
            account => !favouriteList.Exists(fav => fav.Id == account.Id) && account.Id != id).ToList();
    }

    public async Task<string>AddToFavouriteList(int id1, int id2)
    {
        var account = await _db.Accounts.FindAsync(id1);
        var favouriteAccount = await _db.Accounts.FindAsync(id2);
        if (account == null || favouriteAccount == null)
        {
            throw new Exception("Error");
        }
        var favourite = new Favourite();
        favourite.Account = account;
        favourite.FavoriteAccount = favouriteAccount;
        await _db.Favourites.AddAsync(favourite);
        await _db.SaveChangesAsync();
        return "Successfully Added";
    }
}