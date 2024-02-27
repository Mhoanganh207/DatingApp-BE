using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DatingApp.Data;
using DatingApp.Models;
using DatingApp.Repository;
using Microsoft.IdentityModel.Tokens;


namespace DatingApp.Services;

public class AccountService : IAccountService
{
    private readonly AccountRepository _accountRepository = new AccountRepository(new AppDbContext());
    private readonly string _secretkey;
    private readonly IConfiguration _configuration;

    public AccountService(IConfiguration configuration)
    {
        this._configuration = configuration;
        this._secretkey = _configuration.GetSection("AppSettings:Token").Value;
    }
   
    public Task<Account> AddAccount(Account account)
    {
        return _accountRepository.Register(account);
    }

    public Task<Account> GetAccountById(int id)
    {
        return _accountRepository.GetById(id);
    }

    public Task<Account> AccountIsValid(string username, string password)
    {
        return _accountRepository.LogIn(username, password);
    }

    public Task<Account> UpdateAccount(Account account)
    {
        return _accountRepository.UpdateAccount(account);
    }
    public Task<Account> GetAccountByEmail(string email)
    {
        return _accountRepository.GetAccountByEmail(email);
    }

    public string GenerateToken(string email,int id,int time)
    {
        var clamis = new[]
        {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.NameIdentifier, id.ToString())
        };
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(this._secretkey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(clamis),
            Expires = DateTime.Now.AddSeconds(time),
            SigningCredentials = creds
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt =  tokenHandler.WriteToken(token); 

        return jwt;
    }

    public async Task<List<Account>> RetrieveUser(int id,int page)
    {
        return await _accountRepository.GetUser(id,page);
    }
}