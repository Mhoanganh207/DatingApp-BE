using DatingApp.Models;

namespace DatingApp.Services;

public interface IAccountService
{
    Task<Account> AddAccount(Account account);

    Task<Account> GetAccountById(int id);

    Task<bool> AccountIsValid(string username, string password);

    Task<Account> UpdateAvatar(Account account);

    Task<Account> GetAccountByEmail(string email);
}