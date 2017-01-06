using System.Collections.Generic;
using System.Linq;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace AKCore.Controllers
{
    [Route("MemberList")]
    [Authorize]
    public class MemberListController : Controller
    {
        private readonly UserManager<AkUser> _userManager;

        public MemberListController(
            UserManager<AkUser> userManager
            )
        {
            _userManager = userManager;
        }

        public ActionResult Index(MemberListModel model)
        {
            ViewBag.Title = "Adressregister";
            PopulateModel(model);

            return View(model);
        }
        [Route("PostList")]
        public ActionResult PostList()
        {
            ViewBag.Title = "Kamerersposter";
            var model = new PostModel();
            var query=_userManager.Users.Where(x => x.SlavPoster.Length > 2);
            model.Users=new Dictionary<string,IList<AkUser>>();
            foreach (var p in AkPoster.Poster)
            {
                var users=query.Where(x => x.SlavPoster.Contains(p)).ToList();
                model.Users[p]=users;
            }
            return View("PostList",model);
        }

        private void PopulateModel(MemberListModel model)
        {
            var userQuery = _userManager.Users.Where(x=>x.Instrument!=null);
            if (model.SearchPhrase != null)
            {
                var lowerSearch = model.SearchPhrase.ToLower();
                userQuery = userQuery.Where(x => x.FirstName.ToLower().Contains(lowerSearch)
                                                 || x.LastName.ToLower().Contains(lowerSearch)
                                                 || x.Instrument.ToLower().Contains(lowerSearch)
                                                 || x.City.ToLower().Contains(lowerSearch)
                                                 || x.SlavPoster.ToLower().Contains(lowerSearch));
            }
            if (model.Instrument != null)
            {
                userQuery = userQuery.Where(x => x.Instrument == model.Instrument);
            }
            model.Users = userQuery.OrderBy(x=>x.FirstName).GroupBy(x=>x.Instrument).ToList();
        }
    }
}