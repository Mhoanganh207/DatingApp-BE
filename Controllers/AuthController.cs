using System.Security.Claims;
using DatingApp.Models;
using DatingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers;

[Authorize]
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{


    private readonly IAccountService _accountService;

    public AuthController(IAccountService accountService)
    {
        this._accountService = accountService;
    }


    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> AuthenticateAccount(LoginDto loginDto)
    {
        Console.WriteLine(loginDto.Username);
        try
        {
            var auth = await _accountService.AccountIsValid(loginDto.Username, loginDto.Password);
            var jwtToken = _accountService.GenerateToken(loginDto.Username, auth.Id, 7200);
            var refreshToken = _accountService.GenerateToken(loginDto.Username, auth.Id, 604800);
            return Ok(new
            {
                token = jwtToken,
                refreshtoken = refreshToken
            });
        }
        catch (NullReferenceException e)
        {
            return Unauthorized(e);
        }


    }

    [HttpGet("refreshtoken")]
    public IActionResult RefreshToken()
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if(username == null || id == null){
            return Unauthorized();
        }

        var jwtToken = _accountService.GenerateToken(username, Int32.Parse(id), 7200);
        var refreshToken = _accountService.GenerateToken(username, Int32.Parse(id), 604800);
        return Ok(new
        {
            token = jwtToken,
            refreshtoken = refreshToken
        });
    }

}