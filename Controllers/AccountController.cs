using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DatingApp.Models;
using DatingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.Controllers;

[Authorize]
[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IConfiguration _configuration;

    public AccountController(IAccountService accountService, IConfiguration configuration)
    {
        this._accountService = accountService;
        this._configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<Account> RegisterAccount(Account account)
    {
        return await _accountService.AddAccount(account);
    }

    [HttpGet("{id}")]
    public async Task<Account> GetAccountById(int id)
    {
        return await _accountService.GetAccountById(id);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> AuthenticateAccount(LoginDto loginDto)
    {
        var auth = await _accountService.AccountIsValid(loginDto.Username, loginDto.Password);
        if (!auth)
            return Unauthorized();

        var clamis = new[]
        {
            new Claim(ClaimTypes.Name, loginDto.Username)
        };
        string? secretkey = _configuration.GetSection("AppSettings:Token").Value;
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretkey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(clamis),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = creds
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(new
        {
            token = tokenHandler.WriteToken(token)
        });


    }
    
    
    
}