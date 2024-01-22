using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using DatingApp.Models;
using DatingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Account = DatingApp.Models.Account;

namespace DatingApp.Controllers;

[Authorize]
[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IConfiguration _configuration;
    private readonly AmazonS3Client _amazonS3 = new AmazonS3Client(
        "AKIA6AN5YLBFDZKIKPDA",
        "TipoLMZcA4gVLZim9tngRcp1AEkX5fqa+f1mhF3o",
        RegionEndpoint.APSoutheast2
        );
  

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
 
    [AllowAnonymous]
    [HttpPost("{id}/avatar")]
    public async Task<IActionResult> PostAvatar(int id,IFormFile file)
    {
        var account = await this._accountService.GetAccountById(id);
        account.Avatar = file.FileName;
        var request = new PutObjectRequest()
        {
            BucketName = "lovelink",
            InputStream = file.OpenReadStream(),
            Key = file.FileName,
            ContentType = file.ContentType
        };
        try
        {
            var result = await _amazonS3.PutObjectAsync(request);
            await this._accountService.UpdateAvatar(account);
            return Ok(result);

        }
        catch (AmazonS3Exception e)
        {
            throw new AmazonS3Exception(e.Message);
        }
        
    }

    [Authorize]
    [HttpGet("avatar")]
    public async Task<IActionResult> GetAvatar()
    {
        var email = User.FindFirst(ClaimTypes.Name)?.Value;
        if (email == null)
        {
            return StatusCode(500);
        }
        var account = await this._accountService.GetAccountByEmail(email);
        var imageUrl = this._amazonS3.GetPreSignedURLAsync(new GetPreSignedUrlRequest()
        {
            BucketName = "lovelink",
            Key = account.Avatar,
            Expires = DateTime.Now.AddMinutes(1)
        });
        return Ok(imageUrl.Result);
    }
    
    
    
    
    
    
}