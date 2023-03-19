using Newtonsoft.Json;
using System.Collections.Generic;

namespace AKCore.Clients;

public class OpenApiRequestModel
{
    [JsonProperty("model")]
    public string Model { get; set; }
    [JsonProperty("messages")]
    public IEnumerable<OpenApiMessageModel> Messages { get; set; }
    [JsonProperty("temperature")]
    public double Temperature { get; set; }
}
public class OpenApiMessageModel
{
    [JsonProperty("role")]
    public string Role { get; set; }
    [JsonProperty("content")]
    public string Content { get; set; }
}


public class OpenApiResponseModel
{
    [JsonProperty("choices")]
    public IEnumerable<OpenApiChoiseModel> Choices { get; set; }
}
public class OpenApiChoiseModel
{
    [JsonProperty("message")]
    public OpenApiMessageModel Message { get; set; }
}

