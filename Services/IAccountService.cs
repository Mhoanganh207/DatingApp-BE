using DatingApp.Models;

namespace DatingApp.Services;

public interface IAccountService
{
    Task<Account> AddAccount(Account account);

    Task<Account> GetAccountById(int id);

    Task<Account> AccountIsValid(string username, string password);

    Task<Account> UpdateAccount(Account account);

    Task<Account> GetAccountByEmail(string email);

    string GenerateToken(string email,int id,int time);

    Task<List<Account>> RetrieveUser(int id,int page);
}