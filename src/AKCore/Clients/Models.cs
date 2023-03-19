using Newtonsoft.Json;
using System.Collections.Generic;

namespace AKCore.Clients;

public class OpenApiRequestModel
{
    [JsonProperty("model")]
    public string Model { get; set; }
    [JsonProperty("prompt")]
    public string Prompt { get; set; }
    [JsonProperty("temperature")]
    public double Temperature { get; set; }
    [JsonProperty("n")]
    public int Number { get; set; }
    [JsonProperty("max_tokens")]
    public int MaxTokens { get; set; }
}


public class OpenApiResponseModel
{
    [JsonProperty("choices")]
    public IEnumerable<OpenApiChoiseModel> Choices { get; set; }
}
public class OpenApiChoiseModel
{
    [JsonProperty("text")]
    public string Text { get; set; }
}

