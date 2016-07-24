using System.Linq;
using System.Threading.Tasks;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AKCore.Controllers
{
    [Route("User")]
    [Authorize(Roles = "SuperNintendo")]
    public class UserController : Controller
    {
        private readonly UserManager<AkUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(
            UserManager<AkUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<ActionResult> Index(UsersModel model)
        {
            ViewBag.Title = "Användare";
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
        [Route("UserList")]
        public async Task<ActionResult> UserList(UsersModel model)
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
            return PartialView("_UserList", model);
        }

        [Route("CreateUser")]
        public async Task<ActionResult> CreateUser(string userName, string password)
        {
            var newUser = new AkUser {UserName = userName};

            var createRes = await _userManager.CreateAsync(newUser, password);
            if (!createRes.Succeeded)
            {
                return Json(new {success = false, message = createRes.ToString()});
            }

            await _userManager.AddToRoleAsync(newUser, AkRoles.Medlem);

            return Json(new {success = true, message = "Användare skapades"});
        }
        [Route("RemoveUser")]
        public async Task<ActionResult> RemoveUser(string userName)
        {
            var res = await _userManager.FindByNameAsync(userName);
            var delRes = await _userManager.DeleteAsync(res);
            if (delRes.Succeeded)
            {
                return Json(new {success = true, message = "Användare borttagen"});
            }
                return Json(new {success = false, message = delRes.ToString()});
        }
        [Route("AddRole")]
        public async Task<ActionResult> AddRole(string UserName, string Role)
        {
            var user = await _userManager.FindByNameAsync(UserName);
            var role = await _roleManager.FindByNameAsync(Role);
            if(user == null || role == null) { 
                return Json(new { success = false, message = "Misslyckades att lägga till roll" });
            }

            var result=await _userManager.AddToRoleAsync(user, Role);
            if (result.Succeeded)
            {
                return Json(new { success = true });
            }else
            {
                return Json(new { success = false, message = "Misslyckades att lägga till roll" });
            }
        }

        [Route("RemoveRole")]
        public async Task<ActionResult> RemoveRole(string UserName, string Role)
        {
            var user = await _userManager.FindByNameAsync(UserName);
            var role = await _roleManager.FindByNameAsync(Role);
            if (user == null || role == null)
            {
                return Json(new { success = false, message = "Misslyckades att ta bort roll" });
            }

            var result = await _userManager.RemoveFromRoleAsync(user, Role);
            if (result.Succeeded)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = result.ToString() });
            }
        }
    }
}