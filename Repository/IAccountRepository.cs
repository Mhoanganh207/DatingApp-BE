using DatingApp.Models;

namespace DatingApp.Repository;

public interface IAccountRepository
{
    Task<Account> Register(Account account);
    Task<Account> GetById(int id);
    Task<Account> LogIn(string username, string password);
    Task<string> AddToFavouriteList(int id1, int id2);

}