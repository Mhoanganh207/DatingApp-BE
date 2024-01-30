namespace DatingApp.Models;

public class ChatDto
{
  public int AccountId { get; set; }
  public int ChatId { get; set; }
  
  public ChatDto(int account, int chatId)
  {
    AccountId = account;
    ChatId = chatId;
  }
}