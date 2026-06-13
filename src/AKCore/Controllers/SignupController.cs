using System;
using System.Linq;
using AKCore.DataModel;
using AKCore.Models;
using AKCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AKCore.Extensions;
using System.Threading.Tasks;

namespace AKCore.Controllers
{
    [Route("Signup")]
    [Authorize(Roles = "SuperNintendo,Editor")]
    public class SignupController : Controller
    {
        private readonly AKContext _db;
        private readonly RecruitService _recruitService;

        public SignupController(AKContext db, RecruitService recruitService)
        {
            _db = db;
            _recruitService = recruitService;
        }

        [Route("Signup")]
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Signup(JoinUsModel model)
        {
            if(model.BotQuestion != "3")
            {
                return Json(new { success = false, message = "Du angav fel svar för 1 + 2, testa med annan siffra. Ursäkta för krångel, vill bara stoppa spammet." });
            }

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
                _db.Recruits.Any(
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
                Created = DateTime.Now.ConvertToSwedishTime(),
                Email = model.Email,
                Phone = model.Tel,
                Instrument = model.Instrument,
                Other = model.Other
            };
            _db.Recruits.Add(rec);
            _db.SaveChanges();

            return
                Json(new { success = true, message = "Din ansökan är mottagen och vi kommer kontakta dig inom kort" });
        }

        [Route("Recruits")]
        [Authorize(Roles = "SuperNintendo,Editor")]
        public ActionResult Recruits()
        {
            ViewBag.Title = "Anmälningar";
            var model = new RecruitsModel
            {
                Recruits = _db.Recruits
                    .Where(x => x.Created > DateTime.Now.AddMonths(-6))
                    .OrderBy(x => x.Created).ToList()
            };

            return View(model);
        }

        [Route("Archive")]
        [Authorize(Roles = "SuperNintendo,Editor")]
        [HttpPost]
        public async Task<ActionResult> Archive(int id, bool arch)
        {
            var result = await _recruitService.ArchiveAsync(id, arch, User.Identity.Name);
            return Json(new { success = result.Success });
        }

        [Route("Remove")]
        [Authorize(Roles = "SuperNintendo,Editor")]
        [HttpPost]
        public async Task<ActionResult> Remove(int id)
        {
            var result = await _recruitService.RemoveAsync(id, User.Identity.Name);
            return Json(new { success = result.Success });
        }
    }
}
