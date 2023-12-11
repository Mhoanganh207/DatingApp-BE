using DatingApp.Data;
using DatingApp.Models;
using DatingApp.Repository;


namespace DatingApp.Services;

public class AccountService : IAccountService
{
    private readonly AccountRepository _accountRepository = new AccountRepository(new AppDbContext());

  

    public Task<Account> AddAccount(Account account)
    {
        return _accountRepository.Register(account);
    }

    public Task<Account> GetAccountById(int id)
    {
        return _accountRepository.GetById(id);
    }

    public Task<bool> AccountIsValid(string username, string password)
    {
        return _accountRepository.LogIn(username, password);
    }
}