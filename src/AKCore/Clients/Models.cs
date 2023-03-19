using System.Collections.Generic;

namespace AKCore.Clients;

public class OpenApiRequestModel
{
    public string Model { get; set; }
    public IEnumerable<OpenApiMessageModel> Messages { get; set; }
    public double Temperature { get; set; }
}
public class OpenApiMessageModel
{
    public string Role { get; set; }
    public string Content { get; set; }
}


public class OpenApiResponseModel
{
    public IEnumerable<OpenApiChoiseModel> Choises { get; set; }
}
public class OpenApiChoiseModel
{
    public OpenApiMessageModel Message { get; set; }
}

