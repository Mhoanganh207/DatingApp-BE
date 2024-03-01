using DatingApp.Models;
using DatingApp.Services;


namespace DatingApp.Hubs;
using Microsoft.AspNetCore.SignalR;


public class ChatHub : Hub
{
    private static readonly Dictionary<string, string> _userConnections = [];
    
    
    private readonly IChatService _chatService;
    
    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task ActiveUser(string id){
        await Groups.AddToGroupAsync(Context.ConnectionId, id);
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

    public async Task LeaveChat(string chatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
        _userConnections.Remove(Context.ConnectionId);
    }

    
    public async Task SendMessage(string chatId,string sendId, string receiveId ,string message)
    {
        await Clients.Group(chatId).SendAsync("ReceiveMessage", sendId, message);
        await Clients.Group(receiveId).SendAsync("NewMessage", chatId, sendId, message);
        await _chatService.SendMessage(new MessageDto(int.Parse(sendId),int.Parse(chatId),message));
    }
    
   
}