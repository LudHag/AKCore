using System;
using System.Linq;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AKCore.Controllers
{
    [Route("Signup")]
    public class SignupController : Controller
    {
        [Route("Signup")]
        [HttpPost]
        public ActionResult Signup(JoinUsModel model)
        {
            using (var db = new AKContext())
            {
                if (string.IsNullOrWhiteSpace(model.Email) && string.IsNullOrWhiteSpace(model.Tel))
                    return Json(new {success = false, message = "Du har ej angett ett sätt att kontakta dig med."});
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
                    Instrument = model.Instrument
                };
                db.Recruits.Add(rec);
                db.SaveChanges();

                return
                    Json(new {success = true, message = "Din ansökan är mottagen och vi kommer kontakta dig inom kort"});
            }
        }

        [Route("Recruits")]
        [Authorize(Roles = "SuperNintendo,Editor")]
        public ActionResult Recruits()
        {
            ViewBag.Title = "Anmälningar";
            using (var db = new AKContext())
            {
                var model = new RecruitsModel
                {
                    Recruits = db.Recruits.OrderBy(x => x.Created).ToList()
                };

                return View(model);
            }
        }
    }
}