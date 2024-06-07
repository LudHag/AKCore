using Azure.AI.OpenAI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Clients;

public class OpenApiClient
{
    private readonly OpenAIClient _apiClient;
    public OpenApiClient(string token)
    {
        _apiClient =  new OpenAIClient(token);
    }

    public async Task<string> GetText(string query) { 

        var messages = new List<ChatRequestMessage> { 
            new ChatRequestSystemMessage("Only return the response to the question, no additional words."), 
            new ChatRequestUserMessage(query) 
        };

        var chatCompletionsOptions = new ChatCompletionsOptions("gpt-4o", messages);

        var response = await _apiClient.GetChatCompletionsAsync(chatCompletionsOptions);

        return response.Value.Choices[0].Message.Content;
    }

}