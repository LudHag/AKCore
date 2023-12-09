using AKCore.Models;
using AKCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Controllers;

[Route("AlbumEdit")]
[Authorize(Roles = "SuperNintendo,Editor")]
public class AlbumEditController : Controller
{
    private readonly AlbumService _albumService;

    public AlbumEditController(AlbumService albumService)
    {
        _albumService = albumService;
    }

    public ActionResult Index()
    {
        ViewBag.Title = "Album";
        return View();
    }

    [Route("AlbumData")]
    public async Task<ActionResult> AlbumData()
    {
        var albums = await _albumService.GetAllAlbums();
        return Json(albums);
    }

    [Route("AddAlbum")]
    [HttpPost]
    public async Task<ActionResult> AddAlbum(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Json(new { success = false, message = "Fyll i ett namn" });

        try
        {
            var album = await _albumService.AddAlbum(name, User.Identity.Name);
            return Json(new { success = true, id = album.Id, message = "Album skapat" });

        }
        catch (AkValidationError error)
        {
            return Json(new
            {
                success = false,
                message = error.Message
            });
        }
    }

    [HttpPost]
    [Route("DeleteAlbum/{id:int}")]
    public async Task<ActionResult> DeleteAlbum(string id)
    {
        if (!int.TryParse(id, out int aId))
            return Json(new { success = false, message = "Misslyckades med att ta bort album" });

        try
        {
            await _albumService.DeleteAlbum(aId, User.Identity.Name);

            return Json(new { success = true });
        }
        catch (AkValidationError error)
        {
            return Json(new
            {
                success = false,
                message = error.Message
            });
        }
    }

    [HttpPost]
    [Route("UpdateImage")]
    public async Task<ActionResult> UpdateImage(string id, string src)
    {
        if (!int.TryParse(id, out int aId) || string.IsNullOrWhiteSpace(src))
            return Json(new { success = false, message = "Misslyckades med att ändra albumbild" });

        try
        {
            await _albumService.UpdateImage(aId, src, User.Identity.Name);

            return Json(new { success = true });
        }
        catch (AkValidationError error)
        {
            return Json(new
            {
                success = false,
                message = error.Message
            });
        }
    }

    [HttpPost]
    [Route("ChangeName")]
    public async Task<ActionResult> ChangeName(string id, string name)
    {
        if (!int.TryParse(id, out var aId) || string.IsNullOrWhiteSpace(name))
            return Json(new { success = false, message = "Misslyckades med att ändra albumnamn" });
        try
        {

            await _albumService.ChangeName(aId, name, User.Identity.Name);

            return Json(new { success = true });
        }
        catch (AkValidationError error)
        {
            return Json(new
            {
                success = false,
                message = error.Message
            });
        }
    }

    [HttpPost]
    [Route("ChangeCategory")]
    public async Task<ActionResult> ChangeCategory(string id, string category)
    {
        if (!int.TryParse(id, out var aId) || string.IsNullOrWhiteSpace(category))
            return Json(new { success = false, message = "Misslyckades med att ändra albumkategori" });
        try
        {
            await _albumService.ChangeCategory(aId, category, User.Identity.Name);

            return Json(new { success = true });
        }
        catch (AkValidationError error)
        {
            return Json(new
            {
                success = false,
                message = error.Message
            });
        }
    }

    [Route("UploadTracks")]
    public async Task<ActionResult> UploadTracks(AlbumEditModel model)
    {
        if (model.AlbumId < 0)
            return Json(new { success = false, message = "Album not selected" });
        try
        {
            var tracks = await _albumService.UploadTracks(model, User.Identity.Name);
            return
                Json(
                    new { success = true, tracks = JsonConvert.SerializeObject(tracks.OrderBy(x => x.FileName).ToDictionary(x => x.Number)) });
        }
        catch (AkValidationError error)
        {
            return Json(new
            {
                success = false,
                message = error.Message
            });
        }
    }

    [HttpPost]
    [Route("ChangeTrackName")]
    public async Task<ActionResult> ChangeTrackName(string id, string name)
    {
        if (!int.TryParse(id, out var tId))
            return Json(new { success = false, message = "Misslyckades med att ändra namn på spår" });
        if (string.IsNullOrWhiteSpace(name))
        {
            return Json(new { success = false, message = "Misslyckades med att ändra namn på spår" });
        }
        try
        {
            await _albumService.ChangeTrackName(tId, name, User.Identity.Name);

            return Json(new { success = true });
        }
        catch (AkValidationError error)
        {
            return Json(new
            {
                success = false,
                message = error.Message
            });
        }
    }

    [HttpPost]
    [Route("DeleteTrack")]
    public async Task<ActionResult> DeleteTrack(string id, string album)
    {
        if (!int.TryParse(id, out int tId))
            return Json(new { success = false, message = "Misslyckades med att ta bort spår" });
        if (!int.TryParse(album, out int aId))
            return Json(new { success = false, message = "Misslyckades med att ta bort spår" });
        try
        {
            var tracks = await _albumService.DeleteTrack(tId, aId, User.Identity.Name);

            return Json(
                new
                {
                    success = true,
                    tracks = JsonConvert.SerializeObject(tracks.ToDictionary(x => x.Number))
                });
        }
        catch (AkValidationError error)
        {
            return Json(new
            {
                success = false,
                message = error.Message
            });
        }

    }
}