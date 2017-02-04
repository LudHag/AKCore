using System.Collections.Generic;
using System.Linq;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
    }
}