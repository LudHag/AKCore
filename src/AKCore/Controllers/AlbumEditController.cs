using System;
using System.Linq;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AKCore.Controllers
{
    [Route("AlbumEdit")]
    [Authorize(Roles = "SuperNintendo,Editor")]
    public class AlbumEditController : Controller
    {
        private readonly IHostingEnvironment _hostingEnv;

        public AlbumEditController(IHostingEnvironment env)
        {
            _hostingEnv = env;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Album";

            using (var db = new AKContext())
            {
                var model = new AlbumEditModel
                {
                    Albums = db.Albums.Include(x=>x.Tracks).ToList()
                };
                return View(model);
            }
        }

        [Route("AddAlbum")]
        [HttpPost]
        public ActionResult AddAlbum(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Json(new {success = false, message = "Fyll i ett namn"});

            using (var db = new AKContext())
            {
                if (db.Albums.Any(x => x.Name == name))
                    return Json(new {success = false, message = "Ett album med det namnet finns redan"});
                
                var album = new Album
                {
                    Name = name,
                    Created = DateTime.UtcNow
                };
                db.Albums.Add(album);
                db.SaveChanges();

                var filepath = _hostingEnv.WebRootPath + $@"\albums\" + album.Id + @"\";
                System.IO.Directory.CreateDirectory(filepath);

                return Json(new {success = true, id = album.Id});
            }
        }

        [HttpPost]
        [Route("DeleteAlbum/{id:int}")]
        public ActionResult DeleteAlbum(string id)
        {
            var aId = 0;
            if (!int.TryParse(id, out aId))
                return Json(new {success = false, message = "Misslyckades med att ta bort album"});
            using (var db = new AKContext())
            {
                var a = db.Albums.FirstOrDefault(x => x.Id == aId);
                if (a == null) return Json(new {success = false, message = "Misslyckades med att ta bort album"});
                db.Albums.Remove(a);
                db.SaveChanges();

                var filepath = _hostingEnv.WebRootPath + $@"\albums\" + id + @"\";
                System.IO.Directory.Delete(filepath);

                return Json(new {success = true});
            }

        }

        [HttpPost]
        [Route("UpdateImage")]
        public ActionResult UpdateImage(string id, string src)
        {
            var aId = 0;
            if (!int.TryParse(id, out aId) || string.IsNullOrWhiteSpace(src))
                return Json(new {success = false, message = "Misslyckades med att ändra albumbild"});
            using (var db = new AKContext())
            {
                var album = db.Albums.FirstOrDefault(x => x.Id == aId);
                if (album == null)
                    return Json(new {success = false, message = "Misslyckades med att ändra albumbild"});
                album.Image = src;
                db.SaveChanges();

                return Json(new {success = true});
            }
        }

        [HttpPost]
        [Route("ChangeName")]
        public ActionResult ChangeName(string id, string name)
        {
            var aId = 0;
            if (!int.TryParse(id, out aId) || string.IsNullOrWhiteSpace(name))
                return Json(new { success = false, message = "Misslyckades med att ändra albumnamn" });
            using (var db = new AKContext())
            {
                var album = db.Albums.FirstOrDefault(x => x.Id == aId);
                if (album == null)
                    return Json(new { success = false, message = "Misslyckades med att ändra albumnamn" });
                album.Name = name;
                db.SaveChanges();

                return Json(new { success = true });
            }
        }

    }
}
