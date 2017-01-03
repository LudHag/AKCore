using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AKCore.Controllers
{
    [Route("Signup")]
    public class SignupController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Gå med";
            var model = new JoinUsModel();

            return View(model);
        }

        [Route("Signup")]
        [HttpPost]
        public ActionResult Signup(JoinUsModel model)
        {
            using (var db = new AKContext())
            {
                return Json(new {success = true});
            }
        }

        [Route("Signups")]
        [Authorize(Roles = "SuperNintendo")]
        public ActionResult Signups()
        {
            ViewBag.Title = "Anmälningar";

            using (var db = new AKContext())
            {
                return View();
            }
        }
    }
}