using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AKCore.Components
{
    public class MemberListViewComponent : ViewComponent
    {
        private readonly UserManager<AkUser> _userManager;

        public MemberListViewComponent(
            UserManager<AkUser> userManager
        )
        {
            _userManager = userManager;
        }

        public IViewComponentResult Invoke()
        {
            var userQuery = _userManager.Users.Where(x => x.Instrument != null);

            var model = new MemberListModel
            {
                Users = userQuery.OrderBy(x => x.FirstName).GroupBy(x => x.Instrument).ToList()
            };

            return View(model);
        }
    }
}