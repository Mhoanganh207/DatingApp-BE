using System.Security.Claims;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using DatingApp.Models;
using DatingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Account = DatingApp.Models.Account;

namespace DatingApp.Controllers;

[Authorize]
[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly AmazonS3Client _amazonS3 = new AmazonS3Client(
        "AKIA6AN5YLBFDZKIKPDA",
        "TipoLMZcA4gVLZim9tngRcp1AEkX5fqa+f1mhF3o",
        RegionEndpoint.APSoutheast2
        );
  

    public AccountController(IAccountService accountService)
    {
        this._accountService = accountService;
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
        
        var jwtToken = _accountService.GenerateToken(loginDto.Username);
        return Ok(new
        {
            token = jwtToken
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
            Key = file.FileName + account.Email,
            ContentType = file.ContentType
        };
        try
        {
            var result = await _amazonS3.PutObjectAsync(request);
            await this._accountService.UpdateAccount(account);
            return Ok(_accountService.GenerateToken(account.Email));
        }
        catch (AmazonS3Exception e)
        {
            throw new AmazonS3Exception(e.Message);
        }
        
    }

    [AllowAnonymous]
    [HttpPut("{id}/info")]
    public async Task<IActionResult> updateInfor(int id,InforDto inforDto)
    {
        var account = await _accountService.GetAccountById(id);
        account.Interest = inforDto.Interested;
        account.Introduction = inforDto.Introduction;
        await _accountService.UpdateAccount(account);

        return StatusCode(200);
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
            Expires = DateTime.Now.AddDays(7)
        });
        return Ok(imageUrl.Result);
    }
    
    
    
    
    
    
}