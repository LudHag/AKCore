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

    public ExtraInfoController(
       IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    [HttpGet]
    [Route("ProfileData")]
    public async Task<string> GetInfo(string source)
    {

        return "af";
    }
}
