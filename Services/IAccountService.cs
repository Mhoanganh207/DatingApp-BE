using DatingApp.Models;

namespace DatingApp.Services;

public interface IAccountService
{
    void AddAccount(Account account);

    Account GetAccountById(int id);
}