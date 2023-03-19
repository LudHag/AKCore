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

    public async Task<string> GetText()
    {
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri("https://api.openai.com/v1/chat/completions");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var requestBody = GetRequest(@"Write a short description in swedish for an album with these songs: 01 Eye of the Tiger och Svenskt flyg\\r\\n02 Copacabana\\r\\n03 S\\u00E4llskapsresan I\\r\\n04 Dance Ten Looks Three\\r\\n05 S\\u00E4llskapsresan II\\r\\n06 Tivolivisan\\r\\n07 Det b\\u00F6rjar verka k\\u00E4rlek banne mig\\r\\n08 S\\u00E4llskapsresan III\\r\\n09 Stray cat\\r\\n10 S\\u00E4llskapsresan IV\\r\\n11 Bondj\\u00E4vel\\r\\n12 Willie the weeper\\r\\n13 Cantina band\\r\\n14 What a feeling och Fame\\r\\n15 Charlestone\\r\\n16 Balettfobi\\r\\n17 La Cumparasita\\r\\n18 Var ska vi sova i natt\\r\\n19 Fiesta\\r\\n20 The stripper\\r\\n21 Tiger rag\");

            var result = await client.PostAsync("", requestBody);
            var parsedResponse = await GetResponse<OpenApiResponseModel>(result);

            return parsedResponse.Choises.First().Message.Content;

        }

    }

    private StringContent GetRequest(string query)
    {

        var request = new OpenApiRequestModel
        {
            Model = "gpt-3.5-turbo",
            Messages = new List<OpenApiMessageModel>
             {
                 new OpenApiMessageModel
                 {
                     Role = "user",
                     Content = query
                 }
             },
            Temperature = 0.7
        };
        var json = System.Text.Json.JsonSerializer.Serialize(request);
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


//{
//     "model": "gpt-3.5-turbo",
//     "messages": [{"role": "user", "content": "Write a short description in swedish for an album with these songs: 01 Eye of the Tiger och Svenskt flyg\r\n02 Copacabana\r\n03 S\u00E4llskapsresan I\r\n04 Dance Ten Looks Three\r\n05 S\u00E4llskapsresan II\r\n06 Tivolivisan\r\n07 Det b\u00F6rjar verka k\u00E4rlek banne mig\r\n08 S\u00E4llskapsresan III\r\n09 Stray cat\r\n10 S\u00E4llskapsresan IV\r\n11 Bondj\u00E4vel\r\n12 Willie the weeper\r\n13 Cantina band\r\n14 What a feeling och Fame\r\n15 Charlestone\r\n16 Balettfobi\r\n17 La Cumparasita\r\n18 Var ska vi sova i natt\r\n19 Fiesta\r\n20 The stripper\r\n21 Tiger rag"}],
//     "temperature": 0.7
//   }