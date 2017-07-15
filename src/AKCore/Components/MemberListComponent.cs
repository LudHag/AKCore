using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AKCore.Components
{
    public class MemberListViewComponent : ViewComponent
    {
        private readonly UserManager<AkUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public MemberListViewComponent(
            UserManager<AkUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async System.Threading.Tasks.Task<IViewComponentResult> InvokeAsync()
        {
            var role = await _roleManager.FindByNameAsync("Medlem");

            var userQuery = _userManager.Users.Include(u => u.Roles)
                .Where(x => x.Instrument != null)
                .Where(x => x.Roles.Select(z=>z.RoleId).Contains(role.Id));

            var model = new MemberListModel
            {
                Users = userQuery.OrderBy(x => x.FirstName).GroupBy(x => x.Instrument).ToList()
            };

            return View(model);
        }
    }
}