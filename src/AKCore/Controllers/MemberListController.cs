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
            var model = new MemberListModel();
            var query=_userManager.Users.Where(x => x.SlavPoster.Length > 2);
            model.Users=new List<IGrouping<string, AkUser>>();
            foreach (var p in AkPoster.Poster)
            {
                var g = new Grouping<string, AkUser>(p);
                var users=query.Where(x => x.SlavPoster.Contains(p)).ToList();
                foreach (var user in users)
                {
                    g.Add(user);
                }
                
                model.Users.Add(g);
            }
            return View("PostList",model);
        }

        private void PopulateModel(MemberListModel model)
        {
            var userQuery = _userManager.Users.Where(x=>x.Instrument!=null);
            if (model.SearchPhrase != null)
            {
                userQuery = userQuery.Where(x => x.FirstName.Contains(model.SearchPhrase)
                                                 || x.LastName.Contains(model.SearchPhrase)
                                                 || x.Instrument.Contains(model.SearchPhrase)
                                                 || x.City.Contains(model.SearchPhrase)
                                                 || x.SlavPoster.Contains(model.SearchPhrase));
            }
            if (model.Instrument != null)
            {
                userQuery = userQuery.Where(x => x.Instrument == model.Instrument);
            }
            model.Users = userQuery.OrderBy(x=>x.FirstName).GroupBy(x=>x.Instrument).ToList();
        }
    }
}