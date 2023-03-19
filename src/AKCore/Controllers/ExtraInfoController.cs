using AKCore.Clients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace AKCore.Controllers;

[Route("ExtraInfo")]
[Authorize]
public class ExtraInfoController : Controller
{
    private readonly IMemoryCache _memoryCache;
    private readonly OpenApiClient _openApiClient;

    public ExtraInfoController(
       IMemoryCache memoryCache,
       OpenApiClient openApiClient)
    {
        _memoryCache = memoryCache;
        _openApiClient = openApiClient;
    }

    [HttpGet]
    [Route("ProfileData")]
    public async Task<string> GetInfo()
    {

        return await _openApiClient.GetText();
    }
}
