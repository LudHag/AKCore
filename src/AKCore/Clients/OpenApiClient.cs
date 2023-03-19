using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AKCore.Clients;

public class OpenApiClient
{
    private readonly string _token;
    public OpenApiClient(string token)
    {
        _token = token;
    }

    public async Task<string> GetText(IEnumerable<string> queries)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://api.openai.com/v1/chat/completions");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        var requestBody = GetRequest(queries);

        var result = await client.PostAsync("", requestBody);
        var errorBody = await result.Content.ReadAsStringAsync();
        var parsedResponse = await GetResponse<OpenApiResponseModel>(result);

        return parsedResponse.Choices.First().Message.Content.Trim();
    }

    private static StringContent GetRequest(IEnumerable<string> queries)
    {

        var request = new OpenApiRequestModel
        {
            Model = "gpt-3.5-turbo",
            Messages = queries.Select(query => new OpenApiMessageModel
            {
                Role = "user",
                Content = query
            }),
            Temperature = 0.7
        };

        var json = JsonConvert.SerializeObject(request);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private async Task<T> GetResponse<T>(HttpResponseMessage response)
    {
        await using var contentStream = await response.Content.ReadAsStreamAsync();
        using var sr = new StreamReader(contentStream);
        using var jsonTextReader = new JsonTextReader(sr);

        var serializer = new JsonSerializer();

        return serializer.Deserialize<T>(jsonTextReader);
    }
}