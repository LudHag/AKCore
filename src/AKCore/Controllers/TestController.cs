using System.Threading.Tasks;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Hosting;
using System.Linq;

namespace AKCore.Controllers
{
    [Route("Test")]
    public class TestController : Controller
    {
        private readonly AKContext _db;
        public TestController(AKContext db)
        {
            _db = db;
        }
        [Route("Med")]
        public ActionResult Med()
        {
            var pages = _db.Pages.OrderBy(x => x.Name).ToList();
            return View("Med", pages.Count);
        }
        [Route("Utan")]
        public ActionResult Utan()
        {
            return View("Utan");
        }
    }
}