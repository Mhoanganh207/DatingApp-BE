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
    private readonly IFavouriteService _favouriteService;
    private readonly AmazonS3Client _amazonS3 = new AmazonS3Client(
        "AKIA6AN5YLBFDZKIKPDA",
        "TipoLMZcA4gVLZim9tngRcp1AEkX5fqa+f1mhF3o",
        RegionEndpoint.APSoutheast2
        );
  

    public AccountController(IAccountService accountService,IFavouriteService favouriteService)
    {
        this._accountService = accountService;
        this._favouriteService = favouriteService;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<Account> RegisterAccount(Account account)
    {
        return await _accountService.AddAccount(account);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<UserDto> GetAccountById(int id)
    {
        return new UserDto(await _accountService.GetAccountById(id));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> AuthenticateAccount(LoginDto loginDto)
    {
        try
        {
            var auth = await _accountService.AccountIsValid(loginDto.Username, loginDto.Password);
            var jwtToken = _accountService.GenerateToken(loginDto.Username,auth.Id);
            return Ok(new
            {
                token = jwtToken
            });
        }
        catch (NullReferenceException e)
        {
            return Unauthorized();
        }
      


    }
 
    [AllowAnonymous]
    [HttpPost("{id}/avatar")]
    public async Task<IActionResult> PostAvatar(int id,IFormFile file)
    {
        var account = await this._accountService.GetAccountById(id);
        account.Avatar = file.FileName+account.Email;
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
            return Ok(_accountService.GenerateToken(account.Email,account.Id));
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
    


    [AllowAnonymous]
    [HttpGet("{id}/avatar")]
    public async Task<IActionResult> GetAvatarById(int id)
    {
        
        var account = await this._accountService.GetAccountById(id);
        var imageUrl = this._amazonS3.GetPreSignedURLAsync(new GetPreSignedUrlRequest()
        {
            BucketName = "lovelink",
            Key = account.Avatar,
            Expires = DateTime.Now.AddDays(7)
        });
        var client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(imageUrl.Result);
        
        if (response.IsSuccessStatusCode)
        {
            string contentType = response.Content.Headers.ContentType?.MediaType;
            byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
            return File(imageBytes, contentType);
        }

        return StatusCode(500);
    }

    [Authorize]
    [HttpGet("all/{page}")]
    public async Task<List<UserDto>> RetrieveUser(int page)
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;   
        List<UserDto> userList = new List<UserDto>();
        foreach (var user in await _accountService.RetrieveUser(int.Parse(id), page)) 
        {
            userList.Add(new UserDto(user));
        }

        return userList;
    }

    [Authorize]
    [HttpGet("favourite/{id}")]
    public async Task<IActionResult> AddToFavouriteList(int id)
    {
        Console.WriteLine(id);
        var email = User.FindFirst(ClaimTypes.Name)?.Value;
        if (email == null)
        {
            return StatusCode(401);
        }

        var account =await _accountService.GetAccountByEmail(email);
        await _favouriteService.AddFavouriteList(account.Id, id);
        return Ok();
    }
    
    [Authorize]
    [HttpGet("info")]
    public async Task<UserDto> GetFavouriteList()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return new UserDto(await _accountService.GetAccountById(int.Parse(id)));
    }
    
    
    
    
    
    
}