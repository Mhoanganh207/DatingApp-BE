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
   

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
        
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateChat(MessageDto messageDto)
    {
        var id1 = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var id2 = messageDto.ReceiveId;
        var message = messageDto.Content;
        return await _chatService.CreateChat(id1, id2, message);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetChatsById()
    {
        var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        return Ok(new
        {
            ChatList = await _chatService.GetChatsById(id),
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
        var id1 = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        message.SendId = id1;
        return Ok(await _chatService.SendMessage(message));
    }
    
    
   
    
}