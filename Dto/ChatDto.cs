namespace DatingApp.Models;

public class ChatDto
{
  public int AccountId { get; set; }
  public int ChatId { get; set; }

  public long LastMessageId { get; set; }

  public Object LastMessage { get; set; }

}