using System.Security.Claims;
using DatingApp.Models;
using DatingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DatingApp.Controllers;

[Authorize]
[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IFavouriteService _favouriteService;
    private readonly ICacheService _cacheService;


    public ChatController(IChatService chatService, IFavouriteService favouriteService, ICacheService cacheService)
    {
        _chatService = chatService;
        _favouriteService = favouriteService;
        _cacheService = cacheService;
    }
    

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateChat(MessageDto messageDto)
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (idClaim == null)
        {
            return Unauthorized();
        }
        var id1 = int.Parse(idClaim.Value);
        var id2 = messageDto.ReceiveId;
        var message = messageDto.Content;
        await _favouriteService.AddFavouriteList(id1, id2);
        return await _chatService.CreateChat(id1, id2, message);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetChatsById()
    {

        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (idClaim == null)
        {
            return Unauthorized();
        }
        var id = int.Parse(idClaim.Value);
        var data = _cacheService.GetData<List<ChatDto>>("chats");
        if (data != null)
        {
            return Ok(new
            {
                ChatList = data,
                CurrentAccountId = id
            });
        }
        var chatList = await _chatService.GetChatsById(id);
        _cacheService.SetData("chats", chatList, DateTimeOffset.Now.AddSeconds(30));
        return Ok(new
        {
            ChatList = chatList,
            CurrentAccountId = id
        });
    }

    [AllowAnonymous]
    [HttpGet("messages/{id}")]
    public async Task<IActionResult> GetMessagesByChatId(int id)
    {

        return Ok(await _chatService.GetMessagesByChatId(id));
    }

    [Authorize]
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage(MessageDto message)
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (idClaim == null)
        {
            return Unauthorized();
        }
        var id = int.Parse(idClaim.Value);
        message.SendId = id;
        return Ok(await _chatService.SendMessage(message));
    }




}