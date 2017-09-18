using System;
using System.IO;
using System.Linq;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace AKCore.Controllers
{
    [Route("AlbumEdit")]
    [Authorize(Roles = "SuperNintendo,Editor")]
    public class AlbumEditController : Controller
    {
        private static readonly string[] MusicExtensions = {"mp3"};
        private readonly IHostingEnvironment _hostingEnv;
        private readonly AKContext _db;

        public AlbumEditController(IHostingEnvironment env, AKContext db)
        {
            _hostingEnv = env;
            _db = db;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Album";

            var model = new AlbumEditModel
            {
                Albums = _db.Albums.Include(x => x.Tracks).OrderBy(x=>x.Name).ToList()
            };
            return View(model);
        }

        [Route("AddAlbum")]
        [HttpPost]
        public ActionResult AddAlbum(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Json(new {success = false, message = "Fyll i ett namn"});

            if (_db.Albums.Any(x => x.Name == name))
                return Json(new {success = false, message = "Ett album med det namnet finns redan"});

            var album = new Album
            {
                Name = name,
                Created = DateTime.UtcNow
            };
            _db.Albums.Add(album);
            _db.SaveChanges();

            var filepath = _hostingEnv.WebRootPath + $@"\albums\" + album.Id + @"\";
            Directory.CreateDirectory(filepath);

            return Json(new {success = true, id = album.Id});
        }

        [HttpPost]
        [Route("DeleteAlbum/{id:int}")]
        public ActionResult DeleteAlbum(string id)
        {
            if (!int.TryParse(id, out int aId))
                return Json(new { success = false, message = "Misslyckades med att ta bort album" });
            var a = _db.Albums.Include(x=>x.Tracks).FirstOrDefault(x => x.Id == aId);
            if (a == null) return Json(new {success = false, message = "Misslyckades med att ta bort album"});
            foreach (var track in a.Tracks)
            {
                _db.Tracks.Remove(track);
            }

            _db.Albums.Remove(a);

            var filepath = _hostingEnv.WebRootPath + $@"\albums\" + id + @"\";
            Directory.Delete(filepath,true);
            _db.SaveChanges();

            return Json(new {success = true});
        }

        [HttpPost]
        [Route("UpdateImage")]
        public ActionResult UpdateImage(string id, string src)
        {
            if (!int.TryParse(id, out int aId) || string.IsNullOrWhiteSpace(src))
                return Json(new { success = false, message = "Misslyckades med att ändra albumbild" });
            var album = _db.Albums.FirstOrDefault(x => x.Id == aId);
            if (album == null)
                return Json(new {success = false, message = "Misslyckades med att ändra albumbild"});
            album.Image = src;
            _db.SaveChanges();

            return Json(new {success = true});
        }

        [HttpPost]
        [Route("ChangeName")]
        public ActionResult ChangeName(string id, string name)
        {
            if (!int.TryParse(id, out int aId) || string.IsNullOrWhiteSpace(name))
                return Json(new { success = false, message = "Misslyckades med att ändra albumnamn" });
            var album = _db.Albums.FirstOrDefault(x => x.Id == aId);
            if (album == null)
                return Json(new {success = false, message = "Misslyckades med att ändra albumnamn"});
            album.Name = name;
            _db.SaveChanges();

            return Json(new {success = true});
        }

        [Route("UploadTracks")]
        public ActionResult UploadTracks(AlbumEditModel model)
        {
            if (model.AlbumId < 0)
                return Json(new {success = false, message = "Album not selected"});

            var files = model.TrackFiles;
            var album = _db.Albums.Include(x => x.Tracks).FirstOrDefault(x => x.Id == model.AlbumId);
            if (album == null)
                return Json(new {success = false, message = "Album finns ej"});

            foreach (var file in files)
            {
                var filename = ContentDispositionHeaderValue
                    .Parse(file.ContentDisposition)
                    .FileName
                    .ToString()
                    .Trim('"');
                var ext = Path.GetExtension(file.FileName).ToLower();
                if (MusicExtensions.FirstOrDefault(x => ext.EndsWith(x)) == null)
                    return Json(new {success = false, message = "Filen/erna har inte formatet mp3"});

                if (album.Tracks?.FirstOrDefault(x => x.FileName == filename) != null)
                    return Json(new {success = false, message = "Filen finns redan uppladdad"});
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
                    Created = DateTime.UtcNow
                };
                album.Tracks?.Add(track);
            }
            var n = 1;
            foreach (var t in album.Tracks.OrderBy(x => x.FileName))
            {
                t.Number = n;
                n++;
            }
            _db.SaveChanges();
            return
                Json(
                    new {success = true, tracks = JsonConvert.SerializeObject(album.Tracks.OrderBy(x=>x.FileName).ToDictionary(x => x.Number))});
        }

        [HttpPost]
        [Route("ChangeTrackName")]
        public ActionResult ChangeTrackNameAsync(string id, string name)
        {
            if (!int.TryParse(id, out int tId))
                return Json(new { success = false, message = "Misslyckades med att ändra namn på spår" });
            if (string.IsNullOrWhiteSpace(name))
            {
                return Json(new { success = false, message = "Misslyckades med att ändra namn på spår" });
            }

            var track = _db.Tracks.FirstOrDefault(x => x.Id == tId);
            if (track == null) return Json(new { success = false, message = "Misslyckades med att ändra namn på spår" });
            track.Name = name;
            var res = _db.SaveChanges();

            return Json(new { success = res > 0 });
        }


        [HttpPost]
        [Route("DeleteTrack")]
        public ActionResult DeleteTrack(string id, string album)
        {
            if (!int.TryParse(id, out int tId))
                return Json(new { success = false, message = "Misslyckades med att ta bort spår" });
            if (!int.TryParse(album, out int aId))
                return Json(new {success = false, message = "Misslyckades med att ta bort spår"});
            var albumRef = _db.Albums.Include(x => x.Tracks).FirstOrDefault(x => x.Id == aId);
            if (albumRef == null) return Json(new {success = false, message = "Misslyckades med att ta bort spår"});

            var track = albumRef.Tracks.FirstOrDefault(x => x.Id == tId);
            if (track == null) return Json(new {success = false, message = "Misslyckades med att ta bort spår"});
            var trackName = track.FileName;
            _db.Tracks.Remove(track);
            albumRef.Tracks.Remove(track);
            var n = 1;
            foreach (var t in albumRef.Tracks.OrderBy(x=>x.FileName))
            {
                t.Number = n;
                n++;
            }
                
            var filepath = _hostingEnv.WebRootPath + $@"\albums\{album}\{trackName}";
            System.IO.File.Delete(filepath);

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
}