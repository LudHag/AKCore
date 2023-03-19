using Newtonsoft.Json;
using System;
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

    public async Task<string> GetText(string query)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://api.openai.com/v1/completions");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        var requestBody = GetRequest(query);

        var result = await client.PostAsync("", requestBody);
        var errorBody = await result.Content.ReadAsStringAsync();
        var parsedResponse = await GetResponse<OpenApiResponseModel>(result);

        return parsedResponse.Choices.First().Text.Trim();
    }

    private static StringContent GetRequest(string query)
    {

        var request = new OpenApiRequestModel
        {
            Model = "text-davinci-003",
            Prompt = query,
            Temperature = 0.7,
            Number = 1,
            MaxTokens = 240,
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