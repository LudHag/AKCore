using AKCore.DataModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AKCore.Controllers
{
    [Route("Metrics")]
    [Authorize(Roles = "SuperNintendo")]
    public class MetricsController : Controller
    {
        private readonly AKContext _db;

        public MetricsController(
            AKContext db)
        {
            _db = db;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Metrics";
           
            return View();
        }
    }
}

