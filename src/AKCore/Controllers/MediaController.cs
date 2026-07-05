using AKCore.DataModel;
using AKCore.Extensions;
using AKCore.Models;
using AKCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Controllers
{
    [Route("Media")]
    [Authorize(Roles = "SuperNintendo,Editor")]
    public class MediaController : Controller
    {
        private readonly AKContext _db;
        private readonly MediaService _mediaService;

        public MediaController(AKContext db, MediaService mediaService)
        {
            _db = db;
            _mediaService = mediaService;
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
            var page = search.Page < 1 ? 1 : search.Page;
            var medias = PopulateList(page, out int totalPages, search.SearchPhrase ?? "", search.Type, search.Tag);
            if (totalPages < page)
            {
                page = totalPages;
            }

            var model = new MediaModel
            {
                MediaFiles = medias,
                TotalPages = totalPages,
                CurrentPage = page
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
            totalPages = searched.Count().TotalPages(pagesize);
            if (totalPages < page) page = totalPages;
            return searched.Skip((page - 1) * pagesize).Take(pagesize).ToList();
        }
        [Route("EditFile")]
        public async Task<ActionResult> EditFile(string Tag, string Id)
        {
            var result = await _mediaService.EditFileAsync(Tag, Id, User.Identity.Name);
            return Json(new { success = result.Success });
        }

        [Route("UploadFiles")]
        public async Task<ActionResult> UploadFile(MediasModel model)
        {
            var result = await _mediaService.UploadFilesAsync(model, User.Identity.Name);
            return Json(new { success = result.Success, message = result.Message });
        }

        [Route("RemoveFile")]
        public async Task<ActionResult> RemoveFile(string filename)
        {
            var result = await _mediaService.RemoveFileAsync(filename, User.Identity.Name);
            return Json(new { success = result.Success, message = result.Message });
        }
    }
}