using System.Linq;
using System.Threading.Tasks;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace AKCore.Controllers
{
    [Route("User")]
    [Authorize(Roles = "SuperNintendo")]
    public class UserController : Controller
    {
        private readonly UserManager<AkUser> _userManager;

        public UserController(
            UserManager<AkUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ActionResult> Index(UsersModel model)
        {
            var users = _userManager.Users.ToList();
            if (model.SearchPhrase != null)
            {
                users = users.Where(x => x.UserName.Contains(model.SearchPhrase)).ToList();
            }
            foreach (var user in users)
            {
                model.Roles[user.UserName] = await _userManager.GetRolesAsync(user);
            }

            model.Users = users;
            return View(model);
        }
    }
}