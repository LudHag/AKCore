using AKCore.Clients;
using AKCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Controllers;

[Route("ExtraInfo")]
[Authorize]
public class ExtraInfoController : Controller
{
    private readonly IMemoryCache _memoryCache;
    private readonly OpenApiClient _openApiClient;
    private readonly AlbumService _albumService;

    public ExtraInfoController(
       IMemoryCache memoryCache,
       AlbumService albumService,
       OpenApiClient openApiClient)
    {
        _memoryCache = memoryCache;
        _openApiClient = openApiClient;
        _albumService = albumService;
    }

    [HttpGet]
    [Route("GetAlbumInfo")]
    public async Task<string> GetAlbumInfo(int id)
    {
        var hasCache = _memoryCache.TryGetValue<string>($"albuminfo-{id}", out var albumInfo);
        if (hasCache)
        {
            return albumInfo;
        }


        var album = await _albumService.GetAlbum(id);

        var trackNames = string.Join(" ,", album.Tracks.Select(x => x.Name));

        var albumData = @$"Album has name {album.Name} with tracknames: {trackNames}.";
        var query = "How can you describe the album and the type of music featured on its tracks for the listener in swedish?";


        albumInfo = await _openApiClient.GetText(new List<string>()
        {
            albumData,
            query
        });

        _memoryCache.Set($"albuminfo-{id}", albumInfo, TimeSpan.FromDays(5));


        return albumInfo;
    }
}
