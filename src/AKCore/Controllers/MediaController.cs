using System.Collections.Generic;
using System.Linq;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace AKCore.Controllers
{
    [Route("Media")]
    [Authorize(Roles = "SuperNintendo,Editor")]
    public class MediaController : Controller
    {
        private readonly IHostingEnvironment _hostingEnv;

        public MediaController(IHostingEnvironment env)
        {
            _hostingEnv = env;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Filuppladdning";

            int totalPages;
            var medias = PopulateList(1, out totalPages,"");
            var model = new MediaModel
            {
                MediaFiles = medias,
                TotalPages = totalPages,
                CurrentPage = 1
            };
            return View(model);
        }

        [Route("MediaList")]
        public ActionResult MediaList(SearchModel search)
        {
            int totalPages;
            var medias = PopulateList(search.Page<1 ? 1 : search.Page, out totalPages, search.SearchPhrase ?? "");
            if (totalPages < search.Page) search.Page = totalPages;
            var model = new MediaModel
            {
                MediaFiles = medias,
                TotalPages = totalPages,
                CurrentPage = search.Page < 1 ? 1 : search.Page
            };
            return PartialView("_MediaList",model);
        }

        private static List<Media> PopulateList(int page, out int totalPages,string searchPhrase)
        {
            using (var db = new AKContext())
            {
                var searched=db.Medias.Where(x=>x.Name.Contains(searchPhrase));
                totalPages = ((searched.Count()-1) / 8)+1;
                if (totalPages < page) page = totalPages;
                return searched.Skip((page - 1)*8).Take(8).ToList();
            }
        }
        [Route("UploadFile")]
        public ActionResult UploadFile(MediaModel model)
        {
            var file = model.UploadFile;
            var filename = ContentDispositionHeaderValue
                .Parse(file.ContentDisposition)
                .FileName
                .Trim('"');
            var filepath = _hostingEnv.WebRootPath + $@"\media\{filename}";


            using (var db = new AKContext())
            {
                if (db.Medias.FirstOrDefault(x => x.Name == filename) != null)
                {
                    return Json(new {success = false, message = "Filen finns redan uppladdad"});
                }
                using (var fs = System.IO.File.Create(filepath))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
                var mediaFile = new Media
                {
                    Name = filename
                };
                db.Medias.Add(mediaFile);
                db.SaveChanges();
            }

            return Json(new {success = true});
        }
        [Route("RemoveFile")]
        public ActionResult RemoveFile(string filename)
        {
            var filepath = _hostingEnv.WebRootPath + $@"\media\{filename}";

            using (var db = new AKContext())
            {
                var file = db.Medias.FirstOrDefault(x => x.Name == filename);
                if (file == null)
                {
                    return Json(new { success = false, message = "Filen finns ej" });
                }
                try
                {
                    System.IO.File.Delete(filepath);
                }
                catch
                {
                    return Json(new { success = false, message = "Misslyckades ta bort filen" });
                }
                db.Medias.Remove(file);
                db.SaveChanges();
            }

            return Json(new { success = true });
        }
    }
}