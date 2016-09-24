using System;
using System.Linq;
using System.Threading.Tasks;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AKCore.Controllers
{
    [Route("Upcomming")]
    public class UpcommingController : Controller
    {
        private readonly UserManager<AkUser> _userManager;

        public UpcommingController(
            UserManager<AkUser> userManager)
        {
            _userManager = userManager;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "På gång";
            var loggedIn = User.Identity.IsAuthenticated;
            ViewBag.loggedIn = loggedIn;
            using (var db = new AKContext())
            {
                var model = new UpcommingModel
                {
                    Events = db.Events.OrderBy(x => x.Day)
                        .Where(x => loggedIn || (x.Type == "Spelning"))
                        .Where(x => x.Day >= DateTime.UtcNow.Date)
                        .GroupBy(x => x.Day.Year).ToList()
                };

                return View(model);
            }
        }

        [Route("Event/{id:int}")]
        [Authorize]
        public async Task<ActionResult> Event(SignUpModel model, string id)
        {
            ViewBag.Title = "Anmälan";
            var eId = 0;
            if (!int.TryParse(id, out eId))
                return Redirect("/Upcomming");
            using (var db = new AKContext())
            {
                var spelning = db.Events.Include(x => x.SignUps).FirstOrDefault(x => x.Id == eId);
                if (spelning == null) return Redirect("/Upcomming");
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var signup = db.SignUps.FirstOrDefault(x => x.Person == user.UserName);
                if (signup!=null)
                {
                    model.Where = signup.Where;
                    model.Car = signup.Car;
                    model.Instrument = signup.Instrument;
                    model.Comment = signup.Comment;
                }

                model.Event = spelning;

                return View(model);
            }
        }

        [Route("Signup/{id:int}")]
        [Authorize]
        public async Task<ActionResult> SignUp(SignUpModel model, string id)
        {
            var eId = 0;
            if (!int.TryParse(id, out eId))
                return Json(new {success = false, message = "Felaktigt id"});
            using (var db = new AKContext())
            {
                var spelning = db.Events.Include(x => x.SignUps).FirstOrDefault(x => x.Id == eId);
                if (spelning == null) return Json(new {success = false, message = "Felaktigt id"});
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var signup = spelning.SignUps.FirstOrDefault(x => x.Person == user.UserName) ?? new SignUp();
                signup.SignupTime = signup.Where == null ? DateTime.Now : signup.SignupTime;
                signup.Where = model.Where;
                signup.Car = model.Car;
                signup.Instrument = model.Instrument;
                signup.Comment = model.Comment;
                signup.Person = user.UserName;
                signup.PersonName = user.GetName();
                signup.InstrumentName = user.Instrument;
                spelning.SignUps.Add(signup);
                db.SaveChanges();
                return Json(new {success = true});
            }
        }
    }
}