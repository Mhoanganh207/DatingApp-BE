using DatingApp.Data;
using DatingApp.Models;
using DatingApp.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.Services;

public class ChatService : IChatService
{
    private readonly ChatRepository _chatRepository = new ChatRepository(new AppDbContext());
    private readonly AccountRepository _accountRepository = new AccountRepository(new AppDbContext());
    public ChatService()
    {
        
    }

    public async Task<IActionResult> CreateChat(int id1, int id2, string message)
    {
        return await _chatRepository.CreateChat(id1, id2, message);
    }

    public async Task<List<ChatDto>> GetChatsById(int id)
    {
        return await _chatRepository.GetChatsById(id);
    }

    public async Task<List<MessageDto>> GetMessagesByChatId(int id)
    {
        return await _chatRepository.GetMessagesByChatId(id);
    }

    public Task<int> isChatExist(int id1, int id2)
    {
        return _chatRepository.isChatExist(id1, id2);
    }

    public async Task<Message> SendMessage(MessageDto messageDto)
    {
        return await _chatRepository.CreateMessage(messageDto);
    }
  
}