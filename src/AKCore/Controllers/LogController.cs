using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AKCore.Controllers
{
    [Route("Log")]
    [Authorize(Roles = "SuperNintendo")]
    public class LogController : Controller
    {
        private readonly AKContext _db;

        public LogController(
            AKContext db)
        {
            _db = db;
        }

        public ActionResult Index(int p, string user, string type)
        {
            ViewBag.Title = "Logg";
            const int pageSize = 20;
            IQueryable<LogItem> logItems = _db.Log.Include(x => x.ModifiedBy);
            if (!string.IsNullOrWhiteSpace(user))
            {
                logItems = logItems.Where(x => x.ModifiedBy.UserName == user);
            }
            if (!string.IsNullOrWhiteSpace(type))
            {
                logItems = logItems.Where(x => x.Type == type);
            }
            var logItemList = logItems.OrderByDescending(x => x.Modified).Skip(p * pageSize).Take(pageSize).ToList();
            var totalPages = ((logItems.Count() - 1) / pageSize) + 1;
            var model = new LogModel()
            {
                Page = p,
                Type = type,
                UserName = user,
                Log = logItemList,
                TotalPages = totalPages
            };
            return View(model);
        }
    }
}

