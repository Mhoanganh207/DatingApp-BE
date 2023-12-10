using DatingApp.Models;
using DatingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        this._accountService = accountService;
    }

    [HttpPost]
    public void Post(Account account)
    {
        _accountService.AddAccount(account);
    }

    [HttpGet("{id}")]
    public ActionResult<Account> GetAccountById(int id)
    {
        return Ok(_accountService.GetAccountById(id));
    }
    
    
}