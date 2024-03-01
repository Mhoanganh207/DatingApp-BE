using System.Security.Claims;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using DatingApp.Models;
using DatingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
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
        "AKIA6AN5YLBFDPIEKUWH",
        "1K/cgwYCdWrbirQQd8YPW1Ei2r5hlg8JTqx8SkZm",
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
            var token = _accountService.GenerateToken(account.Email,account.Id,86400);
            var refreshtoken = _accountService.GenerateToken(account.Email,account.Id,604800);
            return Ok(new {
                token = token,
                refreshtoken = refreshtoken
            });
        }
        catch (AmazonS3Exception e)
        {
            throw new AmazonS3Exception(e.Message);
        }
        
    }

    [AllowAnonymous]
    [HttpPut("{id}/info")]
    public async Task<IActionResult> UpdateInfor(int id,InforDto inforDto)
    {
        var account = await _accountService.GetAccountById(id);
        account.Interest = inforDto.Interested;
        account.Introduction = inforDto.Introduction;
        await _accountService.UpdateAccount(account);

        return StatusCode(200);
    }

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
        var account = await _accountService.GetAccountById(id);
        var imageUrl = await _amazonS3.GetPreSignedURLAsync(new GetPreSignedUrlRequest()
        {
            BucketName = "lovelink",
            Key = account.Avatar,
            Expires = DateTime.Now.AddDays(7)
        });

        using (var client = new HttpClient())
        {
            var response = await client.GetAsync(imageUrl);

            if (response.IsSuccessStatusCode)
            {
                var contentType = response.Content.Headers.ContentType?.MediaType;
                var imageBytes = await response.Content.ReadAsByteArrayAsync();
                if(contentType == null)
                {
                    return StatusCode(500);
                }
                var fileResult = new FileContentResult(imageBytes, contentType)
                {
                    FileDownloadName = account.Avatar
                };
                Response.Headers["Cache-Control"] = "public, max-age=604800"; 

                return fileResult;
            }
        }

        return StatusCode(500);
    }


    [HttpGet("all/{page}")]
    public async Task<IActionResult> RetrieveUser(int page)
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (id == null){
            return Unauthorized();
        }   
        List<UserDto> userList = [];
        foreach (var user in await _accountService.RetrieveUser(int.Parse(id), page)) 
        {
            userList.Add(new UserDto(user));
        }

        return Ok(userList);
    }


    [HttpGet("favourite/{id}")]
    public async Task<IActionResult> AddToFavouriteList(int id)
    {
        var email = User.FindFirst(ClaimTypes.Name)?.Value;
        if (email == null)
        {
            return StatusCode(401);
        }

        var account =await _accountService.GetAccountByEmail(email);
        await _favouriteService.AddFavouriteList(account.Id, id);
        return Ok();
    }
    
    [HttpGet("info")]
    public async Task<IActionResult> GetFavouriteList()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (id == null)
        {
            return Unauthorized();
        }
        return Ok(new UserDto(await _accountService.GetAccountById(int.Parse(id))));
    }
    
    
    
    
    
    
}