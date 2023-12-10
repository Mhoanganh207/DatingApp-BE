using DatingApp.Data;
using DatingApp.Models;
using DatingApp.Repository;

namespace DatingApp.Services.ServiceImp;

public class AccountService : IAccountService
{
    private readonly AccountRepository _accountRepository = new AccountRepository(new AppDbContext());

  

    public void AddAccount(Account account)
    {
        _accountRepository.Add(account);
    }

    public Account GetAccountById(int id)
    {
        return _accountRepository.GetById(id);
    }
}