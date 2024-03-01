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
        var chats = await (from a in _db.AccountChats
             join b in _db.AccountChats on a.ChatId equals b.ChatId
             join c in _db.Chats on a.ChatId equals c.Id
             where a.AccountId == id && a.AccountId != b.AccountId
             select new ChatDto
             {
                 ChatId = b.ChatId,
                 AccountId = b.AccountId,
                 LastMessageId = c.LastMessageId
             }).OrderByDescending(c => c.LastMessageId).ToListAsync();

        
        foreach (var chat in chats)
        {
            var message = await _db.Messages.FirstOrDefaultAsync(m => m.Id == chat.LastMessageId);
            chat.LastMessage = new
            {
                message.SentId,
                message.Content
            };
        }
        return chats;
    }
    
    public async Task<IActionResult> CreateChat(int id1, int id2, string msg)
    {
        
        var account1 = await _db.Accounts.FindAsync(id1);
        var account2 = await _db.Accounts.FindAsync(id2);
        if (account1 == null || account2 == null)
        {
            throw new Exception("No account found");
        }
        var chat = new Chat();
    
        chat.Accounts.Add(account1);
        chat.Accounts.Add(account2);

        var message = new Message
        {
            Sent = account1,
            Content = msg,
            Chat = chat,
            CreatedAt = DateTime.Now
        };

        await _db.Chats.AddAsync(chat);
        await _db.Messages.AddAsync(message);
        await _db.SaveChangesAsync();
        chat.LastMessageId = message.Id;
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
        var message = new Message
        {
            SentId = msg.SendId,
            Content = msg.Content,
            CreatedAt = DateTime.Now,
            ChatId = msg.ChatId
        };
        await _db.Messages.AddAsync(message);
        await _db.SaveChangesAsync();

        var chat = await _db.Chats.FirstOrDefaultAsync(c => c.Id == msg.ChatId);
        chat.LastMessageId = message.Id;
        await _db.SaveChangesAsync();
        return message;
    }

    
    
   
   
}