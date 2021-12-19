using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace AKCore.Components
{
    public class PostListViewComponent : ViewComponent
    {
        private readonly UserManager<AkUser> _userManager;

        public PostListViewComponent(
            UserManager<AkUser> userManager
        )
        {
            _userManager = userManager;
        }

        public IViewComponentResult Invoke()
        {
            var model = new PostModel();
            var query = _userManager.Users.Where(x => x.SlavPoster.Length > 2);
            model.Users = new Dictionary<string, IList<AkUser>>();
            foreach (var p in AkPoster.Poster)
            {
                var users = query.Where(x => x.SlavPoster.Contains(p)).ToList();
                model.Users[p] = users;
            }
            return View(model);
        }
    }
}