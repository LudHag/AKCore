using AKCore.DataModel;
using AKCore.Extensions;
using AKCore.Models;
using AKCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Controllers;

[Route("AlbumEdit")]
[Authorize(Roles = "SuperNintendo,Editor")]
public class AlbumEditController : Controller
{
    private static readonly string[] MusicExtensions = { "mp3" };
    private readonly IWebHostEnvironment _hostingEnv;
    private readonly AKContext _db;
    private readonly UserManager<AkUser> _userManager;
    private readonly AlbumService _albumService;

    public AlbumEditController(IWebHostEnvironment env, AKContext db, UserManager<AkUser> userManager, AlbumService albumService)
    {
        _hostingEnv = env;
        _db = db;
        _userManager = userManager;
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
        var album = _db.Albums.FirstOrDefault(x => x.Id == aId);
        if (album == null)
            return Json(new { success = false, message = "Misslyckades med att ändra albumnamn" });
        album.Category = category;

        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        _db.Log.Add(new LogItem()
        {
            Type = AkLogTypes.Album,
            Modified = DateTime.Now.ConvertToSwedishTime(),
            ModifiedBy = user,
            Comment = "Album med id " + id + " uppdaterar kategori"
        });

        _db.SaveChanges();

        return Json(new { success = true });
    }

    [Route("UploadTracks")]
    public async Task<ActionResult> UploadTracks(AlbumEditModel model)
    {
        if (model.AlbumId < 0)
            return Json(new { success = false, message = "Album not selected" });

        var files = model.TrackFiles;
        var album = _db.Albums.Include(x => x.Tracks).FirstOrDefault(x => x.Id == model.AlbumId);
        if (album == null)
            return Json(new { success = false, message = "Album finns ej" });

        foreach (var file in files)
        {
            var filename = ContentDispositionHeaderValue
                .Parse(file.ContentDisposition)
                .FileName
                .ToString()
                .Trim('"');
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (MusicExtensions.FirstOrDefault(x => ext.EndsWith(x)) == null)
                return Json(new { success = false, message = "Filen/erna har inte formatet mp3" });

            if (album.Tracks?.FirstOrDefault(x => x.FileName == filename) != null)
                return Json(new { success = false, message = "Filen finns redan uppladdad" });
        }

        foreach (var file in files)
        {
            var filename = ContentDispositionHeaderValue
                .Parse(file.ContentDisposition)
                .FileName
                .ToString()
                .Trim('"');
            var filepath = _hostingEnv.WebRootPath + $@"\albums\{model.AlbumId}\{filename}";

            using (var fs = System.IO.File.Create(filepath))
            {
                file.CopyTo(fs);
                fs.Flush();
            }
            var track = new Track
            {
                FileName = filename,
                Created = DateTime.Now.ConvertToSwedishTime()
            };
            album.Tracks?.Add(track);
        }
        var n = 1;
        foreach (var t in album.Tracks.OrderBy(x => x.FileName))
        {
            t.Number = n;
            n++;
        }

        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        _db.Log.Add(new LogItem()
        {
            Type = AkLogTypes.Album,
            Modified = DateTime.Now.ConvertToSwedishTime(),
            ModifiedBy = user,
            Comment = "Album med id " + model.AlbumId + " laddar upp filer"
        });

        _db.SaveChanges();
        return
            Json(
                new { success = true, tracks = JsonConvert.SerializeObject(album.Tracks.OrderBy(x => x.FileName).ToDictionary(x => x.Number)) });
    }

    [HttpPost]
    [Route("ChangeTrackName")]
    public async Task<ActionResult> ChangeTrackNameAsync(string id, string name)
    {
        if (!int.TryParse(id, out var tId))
            return Json(new { success = false, message = "Misslyckades med att ändra namn på spår" });
        if (string.IsNullOrWhiteSpace(name))
        {
            return Json(new { success = false, message = "Misslyckades med att ändra namn på spår" });
        }

        var track = _db.Tracks.FirstOrDefault(x => x.Id == tId);
        if (track == null) return Json(new { success = false, message = "Misslyckades med att ändra namn på spår" });
        track.Name = name;

        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        _db.Log.Add(new LogItem()
        {
            Type = AkLogTypes.Album,
            Modified = DateTime.Now.ConvertToSwedishTime(),
            ModifiedBy = user,
            Comment = "Musikspår med id " + id + " byter namn"
        });

        var res = _db.SaveChanges();

        return Json(new { success = res > 0 });
    }


    [HttpPost]
    [Route("DeleteTrack")]
    public async Task<ActionResult> DeleteTrack(string id, string album)
    {
        if (!int.TryParse(id, out int tId))
            return Json(new { success = false, message = "Misslyckades med att ta bort spår" });
        if (!int.TryParse(album, out int aId))
            return Json(new { success = false, message = "Misslyckades med att ta bort spår" });
        var albumRef = _db.Albums.Include(x => x.Tracks).FirstOrDefault(x => x.Id == aId);
        if (albumRef == null) return Json(new { success = false, message = "Misslyckades med att ta bort spår" });

        var track = albumRef.Tracks.FirstOrDefault(x => x.Id == tId);
        if (track == null) return Json(new { success = false, message = "Misslyckades med att ta bort spår" });
        var trackName = track.FileName;
        _db.Tracks.Remove(track);
        albumRef.Tracks.Remove(track);
        var n = 1;
        foreach (var t in albumRef.Tracks.OrderBy(x => x.FileName))
        {
            t.Number = n;
            n++;
        }

        var filepath = _hostingEnv.WebRootPath + $@"\albums\{album}\{trackName}";
        System.IO.File.Delete(filepath);

        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        _db.Log.Add(new LogItem()
        {
            Type = AkLogTypes.Album,
            Modified = DateTime.Now.ConvertToSwedishTime(),
            ModifiedBy = user,
            Comment = "Musikspår med id " + id + " tas bort"
        });

        _db.SaveChanges();

        return
            Json(
                new
                {
                    success = true,
                    tracks = JsonConvert.SerializeObject(albumRef.Tracks.ToDictionary(x => x.Number))
                });
    }
}