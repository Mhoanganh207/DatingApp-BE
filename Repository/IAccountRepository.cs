using DatingApp.Models;

namespace DatingApp.Repository;

public interface IAccountRepository
{
    Task<Account> Register(Account account);
    Task<Account> GetById(int id);
    Task<bool> LogIn(string username, string password);

}