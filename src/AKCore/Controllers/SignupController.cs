using System;
using System.Linq;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace AKCore.Controllers
{
    [Route("Signup")]
    [Authorize(Roles = "SuperNintendo,Editor")]
    public class SignupController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public SignupController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [Route("Signup")]
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Signup(JoinUsModel model)
        {
            using (var db = new AKContext(_hostingEnvironment))
            {
                if (string.IsNullOrWhiteSpace(model.Email) && string.IsNullOrWhiteSpace(model.Tel))
                    return Json(new { success = false, message = "Du har ej angett ett sätt att kontakta dig med." });
                if (string.IsNullOrWhiteSpace(model.Instrument))
                    return
                        Json(
                            new
                            {
                                success = false,
                                message =
                                "Du måste ange vilke instrument du spelar eller om du vill dansa med baletten."
                            });
                if (
                    db.Recruits.Any(
                        x =>
                            (!string.IsNullOrWhiteSpace(model.Email) && (x.Email == model.Email)) ||
                            (!string.IsNullOrWhiteSpace(model.Tel) && (x.Phone == model.Tel))))
                    return
                        Json(
                            new
                            {
                                success = false,
                                message = "En person med din kontaktinformation har redan anmält sig."
                            });

                var rec = new Recruit
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Created = DateTime.UtcNow,
                    Email = model.Email,
                    Phone = model.Tel,
                    Instrument = model.Instrument,
                    Other = model.Other
                };
                db.Recruits.Add(rec);
                db.SaveChanges();

                return
                    Json(new { success = true, message = "Din ansökan är mottagen och vi kommer kontakta dig inom kort" });
            }
        }

        [Route("Recruits")]
        [Authorize(Roles = "SuperNintendo,Editor")]
        public ActionResult Recruits()
        {
            ViewBag.Title = "Anmälningar";
            using (var db = new AKContext(_hostingEnvironment))
            {
                var model = new RecruitsModel
                {
                    Recruits = db.Recruits
                        .Where(x => x.Created > DateTime.Now.AddMonths(-6))
                        .OrderBy(x => x.Created).ToList()
                };

                return View(model);
            }
        }

        [Route("Archive")]
        [Authorize(Roles = "SuperNintendo,Editor")]
        [HttpPost]
        public ActionResult Archive(int id, bool arch)
        {
            if (id < 0)
            {
                return Json(new { success = false });
            }
            using (var db = new AKContext(_hostingEnvironment))
            {
                var recruit = db.Recruits.FirstOrDefault(x => x.Id == id);
                if (recruit == null) return Json(new { success = false });
                recruit.Archived = arch;
                db.SaveChanges();
                return Json(new { success = true });
            }
        }
        [Route("Remove")]
        [Authorize(Roles = "SuperNintendo,Editor")]
        [HttpPost]
        public ActionResult Remove(int id)
        {
            if (id < 0)
            {
                return Json(new { success = false });
            }
            using (var db = new AKContext(_hostingEnvironment))
            {
                var recruit = db.Recruits.FirstOrDefault(x => x.Id == id);
                if (recruit == null) return Json(new { success = false });
                db.Recruits.Remove(recruit);
                db.SaveChanges();
                return Json(new { success = true });
            }
        }
    }
}
