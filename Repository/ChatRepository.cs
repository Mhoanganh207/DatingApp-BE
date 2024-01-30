using DatingApp.Data;
using DatingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Repository;

public class ChatRepository 
{
    private readonly AppDbContext _db ;

    public ChatRepository(AppDbContext db)
    {
        this._db = db;
    }
    
    public async Task<int> isChatExist(int id1, int id2)
    {
        var chat = await _db.Chats.FirstOrDefaultAsync(c => c.Accounts.Contains(new Account(){Id = id1}) && c.Accounts.Contains(new Account(){Id = id2}));
        if (chat == null)
        {
            return -1;
        }
        return chat.Id;
    }
    
    public async Task<List<ChatDto>> GetChatsById(int id)
    {
        var sqlQuery = @"
          SELECT b.*
          FROM AccountChats AS a
          JOIN AccountChats AS b ON a.ChatId = b.ChatId AND a.AccountId != b.AccountId
          WHERE a.AccountId = {0}";

        var chats = _db.AccountChats
            .FromSqlRaw(sqlQuery, id)
            .ToList();

        var accounts = new List<ChatDto>();
        foreach (var ac in chats)
        {
          accounts.Add(new ChatDto(ac.AccountId, ac.ChatId));
        }
       
        
        return accounts;
    }
    
    public async Task<IActionResult> CreateChat(int id1, int id2, string message)
    {
        
        var account1 = await _db.Accounts.FindAsync(id1);
        var account2 = await _db.Accounts.FindAsync(id2);
        if (account1 == null || account2 == null)
        {
            throw new Exception("No account found");
        }
        var chat = new Chat();
        var message1 = new Message();
        chat.Accounts.Add(account1);
        chat.Accounts.Add(account2);
        message1.Sent = account1;
        message1.Content = message;
        message1.Chat = chat;
        message1.CreatedAt = DateTime.Now;
        
        await _db.Chats.AddAsync(chat);
        await _db.Messages.AddAsync(message1);
        await _db.SaveChangesAsync();
        return new OkResult();
    }
    
    public async Task<List<MessageDto>> GetMessagesByChatId(int id)
    {
        var chat = await _db.Messages.Where(m => m.Chat.Id == id).ToListAsync();
        var list = new List<MessageDto>();
        foreach (var message in chat)
        {
            list.Add(new MessageDto(message.SentId,id,message.Content));
        }
        return list;

    }
    
    public async Task<Message> CreateMessage(MessageDto msg)
    {
        var message1 = new Message();
        message1.SentId = msg.SendId;
        message1.Content = msg.Content;
        message1.CreatedAt = DateTime.Now;
        message1.ChatId = msg.ChatId;
        await _db.Messages.AddAsync(message1);
        await _db.SaveChangesAsync();
        return message1;
    }
    
   
   
}