using DatingApp.Models;
using DatingApp.Services;


namespace DatingApp.Hubs;
using Microsoft.AspNetCore.SignalR;


public class ChatHub : Hub
{
    private static Dictionary<string, string> _userConnections = new Dictionary<string, string>();
    
    private readonly IChatService _chatService;
    
    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }
    

    public async Task JoinChat(string chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        _userConnections.Add(Context.ConnectionId, chatId);
    }
    
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    
    public async Task SendMessage(string chatId,string sendId ,string message)
    {
        await Clients.Group(chatId).SendAsync("ReceiveMessage", sendId, message);
        await _chatService.SendMessage(new MessageDto(int.Parse(sendId),int.Parse(chatId),message));
    }
    
   
}