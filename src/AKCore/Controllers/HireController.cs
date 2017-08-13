using System;
using System.Linq;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
/**
namespace AKCore.Controllers
{
    [Route("Hire")]
    [Authorize(Roles = "SuperNintendo")]
    public class HireController : Controller
    {
        private readonly IHostingEnvironment _hostingEnv;

        public HireController(IHostingEnvironment env)
        {
            _hostingEnv = env;
        }
        [Route("Hire")]
        [HttpPost]
        public ActionResult Hire(HireModel model)
        {
            using (var db = new AKContext(_hostingEnv))
            {
                if (string.IsNullOrWhiteSpace(model.Email) && string.IsNullOrWhiteSpace(model.Tel))
                    return Json(new { success = false, message = "Du har ej angett ett sätt att kontakta dig med." });
                if (string.IsNullOrWhiteSpace(model.Name))
                    return
                        Json(
                            new
                            {
                                success = false,
                                message =
                                "Du måste anget ditt namn."
                            });

                var hir = new Hire
                {
                    Name = model.Name,
                    Email = model.Email,
                    Created = DateTime.UtcNow,
                    Tel = model.Tel,
                    Other = model.Other
                };
                db.Hires.Add(hir);
                db.SaveChanges();

                return
                    Json(new { success = true, message = "Din förfrågan är mottagen och vi kommer kontakta dig inom kort" });
            }
        }

        [Route("Hires")]
        [Authorize(Roles = "SuperNintendo,Editor")]
        public ActionResult Hires()
        {
            ViewBag.Title = "Spelningsförfrågningar";
            using (var db = new AKContext(_hostingEnv))
            {

                var model = new HiresModel()
                {
                    Hires = db.Hires
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
            using (var db = new AKContext(_hostingEnv))
            {
                var hire = db.Hires.FirstOrDefault(x => x.Id == id);
                if (hire == null) return Json(new { success = false });
                hire.Archived = arch;
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
            using (var db = new AKContext(_hostingEnv))
            {
                var hire = db.Hires.FirstOrDefault(x => x.Id == id);
                if (hire == null) return Json(new { success = false });
                db.Hires.Remove(hire);
                db.SaveChanges();
                return Json(new { success = true });
            }
        }

    }
}
    */