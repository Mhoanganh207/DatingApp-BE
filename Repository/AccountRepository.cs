using DatingApp.Data;
using DatingApp.Models;

namespace DatingApp.Repository;

public class AccountRepository
{
    private readonly AppDbContext _db ;

    public AccountRepository(AppDbContext db)
    {
        this._db = db;
    }

    public void Add(Account account)
    {
        _db.Accounts.Add(account);
        _db.SaveChanges();
    }

    public Account GetById(int id)
    {
        var account = _db.Accounts.Find(id);
        if (account == null)
        {
            throw new NullReferenceException("Cannot find account");
        }
        else
        {
            return account;
        }
    }

}