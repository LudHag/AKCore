using Azure.AI.OpenAI;
using OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Clients;

public class OpenApiClient
{
    private readonly ChatClient _chatClient;
    public OpenApiClient(string token)
    {
        var apiClient = new OpenAIClient(token);
        _chatClient = apiClient.GetChatClient("gpt-4o");
    }

    public async Task<string> GetText(string query, string imageUrl = null)
    {

        var messages = new List<ChatMessage> {
            ChatMessage.CreateSystemMessage("Only return the response to the question, no additional words."),
            imageUrl == null ?
             ChatMessage.CreateUserMessage(query) :
             ChatMessage.CreateUserMessage(ChatMessageContentPart.CreateTextMessageContentPart(query), ChatMessageContentPart.CreateImageMessageContentPart(new Uri(imageUrl)))
        };

        var response = await _chatClient.CompleteChatAsync(messages);

        return response.Value.Content[0].Text;
    }

}