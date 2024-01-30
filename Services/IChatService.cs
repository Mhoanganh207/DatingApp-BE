using DatingApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Services;

public interface IChatService
{
    Task<IActionResult> CreateChat(int id1, int id2, string message);
    Task<List<ChatDto>> GetChatsById(int id);
    Task<List<MessageDto>> GetMessagesByChatId(int id);
    Task<int> isChatExist(int id1, int id2);
    Task<Message> SendMessage(MessageDto messageDto);
}