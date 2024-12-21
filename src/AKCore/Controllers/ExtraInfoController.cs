using AKCore.Clients;
using AKCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Controllers;

[Route("ExtraInfo")]
[Authorize]
public class ExtraInfoController(
   IMemoryCache memoryCache,
   AlbumService albumService,
   OpenApiClient openApiClient) : Controller
{


    [HttpGet]
    [Route("GetAlbumInfo")]
    public async Task<ActionResult> GetAlbumInfo(int id)
    {
        var hasCache = memoryCache.TryGetValue<string>($"albuminfo-{id}", out var albumInfo);
        if (hasCache)
        {
            return Json(new { albumInfo });
        }


        var album = await albumService.GetAlbum(id);

        var trackNames = string.Join("\\n", album.Tracks.Select(x => x.Number + ". " + (x.FileName ?? x.Name)));

        var query = $@"Please provide a short description in swedish describing about the following 
                        album titled {album.Name} mentioning the attached albumcover and giving a 
                        short description of a couple of its tracks(try to clean up track name so its not just filenames) metioned below with original artists metioned:
                        {trackNames}";

        albumInfo = await openApiClient.GetText(query, "https://www.altekamereren.org/" + album.Image);

        memoryCache.Set($"albuminfo-{id}", albumInfo, TimeSpan.FromDays(5));


        return Json(new { albumInfo });
    }

    [HttpPost]
    [Route("TranslateHtml")]
    public async Task<ActionResult> GetTranslatedHtml(string text)
    {

        var query = $@"Please translate the following html from swedish to english:
                    {text}";

        var data = (await openApiClient.GetText(query)).Trim();



        return Json(new { success = true, data });
    }

    [HttpPost]
    [Route("TranslateText")]
    public async Task<ActionResult> GetTranslatedText(string text)
    {

        var query = $@"Please translate the following text from swedish to english:
                    {text}";

        var data = (await openApiClient.GetText(query)).Trim();

        return Json(new { success = true, data });
    }
}
