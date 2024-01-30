namespace DatingApp.Models;

public class MessageDto
{
    public int ChatId { get; set; }
    public string Content { get; set; }
    public int SendId { get; set; }
    public int ReceiveId { get; set; }
   
    public MessageDto(int sendId, int chatId, string content)
    {
        Content = content;
        ChatId = chatId;
        SendId = sendId;
    }
}