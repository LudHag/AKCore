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
    public async Task<ActionResult> GetAlbumInfo(int id)
    {
        var hasCache = _memoryCache.TryGetValue<string>($"albuminfo-{id}", out var albumInfo);
        if (hasCache)
        {
            return Json(new { albumInfo });
        }


        var album = await _albumService.GetAlbum(id);

        var trackNames = string.Join("\\n", album.Tracks.Select(x => x.Number + ". " + (x.FileName ?? x.Name)));

        var query = $@"Please provide a short description in swedish describing notable tracks and themes for the album titled {album.Name} with the following tracklist:
                        {trackNames}";

        albumInfo = await _openApiClient.GetText(new List<string>()
        {
            query
        });

        _memoryCache.Set($"albuminfo-{id}", albumInfo, TimeSpan.FromDays(5));


        return Json(new { albumInfo });
    }

    [HttpPost]
    [Route("Translate")]
    public async Task<ActionResult> GetTranslatedText(string text)
    {

        var query = $@"Please translate the following html from swedish to english:
                    {text}";

        var data = (await _openApiClient.GetText(new List<string>()
        {
            query
        })).Trim();



        return Json(new { success = true, data });
    }
}
