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

    public async Task<string> GetText(IEnumerable<string> queries) { 

        var allMessages = new List<ChatRequestMessage> { new ChatRequestSystemMessage("Only return the response to the question, no additional words.") };
        allMessages.AddRange(queries.Select(x => new ChatRequestUserMessage(x)));
        var chatCompletionsOptions = new ChatCompletionsOptions("gpt-4o", allMessages);

        var response = await _apiClient.GetChatCompletionsAsync(chatCompletionsOptions);

        return response.Value.Choices[0].Message.Content;
    }

}