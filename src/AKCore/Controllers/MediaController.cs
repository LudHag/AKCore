using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AKCore.Controllers
{
    [Route("Media")]
    [Authorize(Roles = "SuperNintendo,Editor")]
    public class MediaController : Controller
    {
        private static readonly string[] ImageExtensions = { "jpg", "bmp", "gif", "png", "svg" };
        private static readonly string[] DocumentExtensions = { "pdf", "docx", "doc" };
        private readonly AKContext _db;
        private readonly IWebHostEnvironment _hostingEnv;

        public MediaController(AKContext db, IWebHostEnvironment hostingEnv)
        {
            _db = db;
            _hostingEnv = hostingEnv;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Filuppladdning";
            return View();
        }

        [Route("MediaData")]
        public ActionResult MediaData()
        {
            var model = _db.Medias
                .ToList()
                .GroupBy(x => x.Tag).ToDictionary(x => x.Key);

            return Json(model);
        }

        [Route("MediaPickerList")]
        public ActionResult MediaPickerList(SearchModel search)
        {
            var medias = PopulateList(search.Page < 1 ? 1 : search.Page, out int totalPages, search.SearchPhrase ?? "", search.Type, search.Tag);
            if (totalPages < search.Page)
            {
                search.Page = totalPages;
            }

            var model = new MediaModel
            {
                MediaFiles = medias,
                TotalPages = totalPages,
                CurrentPage = search.Page < 1 ? 1 : search.Page
            };
            return PartialView("_MediaPickerList", model);
        }

        [HttpGet("ImageListData")]
        public ActionResult MediaPickerListData()
        {
            var images = _db.Medias
                .Where(x => x.Type == "Image")
                .OrderByDescending(x => x.Created);

            return Json(images);
        }

        [HttpGet("DocumentListData")]
        public ActionResult DocumentPickerListData()
        {
            var images = _db.Medias
                .Where(x => x.Type == "Document")
                .OrderByDescending(x => x.Created);

            return Json(images);
        }

        private List<Media> PopulateList(int page, out int totalPages, string searchPhrase, string type = "", string tag = "")
        {
            var pagesize = 12;
            var searched = _db.Medias
                .Where(x => x.Name.Contains(searchPhrase))
                .Where(x => string.IsNullOrWhiteSpace(type) || x.Type == type)
                .Where(x => string.IsNullOrWhiteSpace(tag) || x.Tag == tag)
                .OrderByDescending(x => x.Created);
            totalPages = ((searched.Count() - 1) / pagesize) + 1;
            if (totalPages < page)
            {
                page = totalPages;
            }

            return searched.Skip((page - 1) * pagesize).Take(pagesize).ToList();
        }
        [Route("EditFile")]
        public ActionResult EditFile(string Tag, string Id)
        {
            if (int.TryParse(Id, out int iId) && !string.IsNullOrWhiteSpace(Tag))
            {
                var file = _db.Medias.FirstOrDefault(x => x.Id == iId);
                if (file != null)
                {
                    file.Tag = Tag;
                    _db.SaveChanges();
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
        [Route("UploadFiles")]
        public ActionResult UploadFile(MediasModel model)
        {
            foreach (var uploadFile in model.UploadFiles)
            {

                var file = uploadFile;
                var ext = Path.GetExtension(file.FileName).ToLower();
                var isImage = ImageExtensions.FirstOrDefault(x => ext.EndsWith(x)) != null;
                var isDocument = DocumentExtensions.FirstOrDefault(x => ext.EndsWith(x)) != null;
                if (!(isImage || isDocument) || string.IsNullOrWhiteSpace(model.Tag))
                {
                    return Json(new { success = false, message = "Filen har fel format" });
                }

                var filename = ContentDispositionHeaderValue
                    .Parse(file.ContentDisposition)
                    .FileName
                    .ToString()
                    .Trim('"');
                var filepath = _hostingEnv.WebRootPath + $@"\media\{filename}";

                if (_db.Medias.FirstOrDefault(x => x.Name == filename) != null)
                {
                    return Json(new { success = false, message = "Filen finns redan uppladdad" });
                }
                using (var fs = System.IO.File.Create(filepath))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
                var mediaFile = new Media
                {
                    Name = filename,
                    Created = DateTime.Now,
                    Tag = model.Tag
                };
                mediaFile.Type = isImage ? "Image" : "Document";
                _db.Medias.Add(mediaFile);
            }

            _db.SaveChanges();

            return Json(new { success = true });
        }
        [Route("RemoveFile")]
        public ActionResult RemoveFile(string filename)
        {
            var filepath = _hostingEnv.WebRootPath + $@"\media\{filename}";

            var file = _db.Medias.FirstOrDefault(x => x.Name == filename);
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
            _db.Medias.Remove(file);
            _db.SaveChanges();

            return Json(new { success = true });
        }
    }
}