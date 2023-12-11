using Microsoft.AspNetCore.Mvc;
using OpenAI_API;


namespace DatingApp.Controllers;

[ApiController]
[Route("api/chatbot")]
public class ChatBotController
{
    private readonly OpenAIAPI _api = new OpenAI_API.OpenAIAPI("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");

    [HttpGet]
    public async void Demo(string ques)
    {
        var conversation = _api.Chat.CreateConversation();
        conversation.AppendUserInput("She said she didn't love my anymore");

        await conversation.StreamResponseFromChatbotAsync(res =>
        {
           Console.WriteLine(res);
        });

    }
    
}