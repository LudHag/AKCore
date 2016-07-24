using System.IO;
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
            var model=new MediaModel();
            return View(model);
        }

        [Route("UploadFile")]
        public ActionResult UploadFile(MediaModel model)
        {
            var file = model.UploadFile;
            var filename = ContentDispositionHeaderValue
                            .Parse(file.ContentDisposition)
                            .FileName
                            .Trim('"');
            var filepath= _hostingEnv.WebRootPath + $@"\media\{filename}";

            
            using (var db = new AKContext())
            {
                if (db.Medias.FirstOrDefault(x => x.Name == filename) != null)
                {
                    return Json(new { success = false, message = "Filen finns redan uppladdad" });
                }
                using (var fs = System.IO.File.Create(filepath))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
                var mediaFile = new Media()
                {
                    Name = filename
                };
                db.Medias.Add(mediaFile);
                db.SaveChanges();
            }

            return Json(new { success = true });
        }
    }
}