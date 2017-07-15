using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AKCore.Controllers
{
    [Route("User")]
    [Authorize(Roles = "SuperNintendo")]
    public class UserController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AkUser> _userManager;

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
            await PopulateModel(model);
            return View(model);
        }

        [Route("UserList")]
        public async Task<ActionResult> UserList(UsersModel model)
        {
            await PopulateModel(model);
            return PartialView("_UserList", model);
        }

        private async Task PopulateModel(UsersModel model)
        {
            var users = _userManager.Users.ToList();
            if (model.SearchPhrase != null)
                users =
                    users.Where(
                        x =>
                            x.UserName.Contains(model.SearchPhrase) ||
                            (x.FirstName + ' ' + x.LastName).Contains(model.SearchPhrase)).OrderBy(x=>x.FirstName).ToList();
            foreach (var user in users)
            {
                model.Roles[user.UserName] = await _userManager.GetRolesAsync(user);
                model.Posts[user.UserName] = user.SlavPoster != null
                    ? JsonConvert.DeserializeObject<List<string>>(user.SlavPoster)
                    : new List<string>();
            }

            model.Users = users;
        }

        [Route("CreateUser")]
        public async Task<ActionResult> CreateUser(ProfileModel model)
        {
            if(string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
            {
                return Json(new { success = false, message = "Användarnamn och lösenord krävs" });
            }
            var oldUser=await _userManager.FindByNameAsync(model.UserName);
            if (oldUser!=null)
            {
                return Json(new { success = false, message = "Användarnamn finns redan" });
            }

            var newUser = new AkUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Adress = model.Adress,
                ZipCode = model.ZipCode,
                City = model.City,
                Phone = model.Phone,
                Nation = model.Nation,
                Instrument = model.Instrument,
                SlavPoster = model.Poster ==null ? "" : JsonConvert.SerializeObject(model.Poster),
                Medal = model.Medal
            };
            
            var createRes = await _userManager.CreateAsync(newUser, model.Password);

            if (!createRes.Succeeded)
            {
                return Json(new { success = false, message = string.Join(" ", createRes.ToString()) });
            }

            if (model.Roles != null)
            {
                foreach(var role in model.Roles)
                {
                    if (!string.IsNullOrWhiteSpace(role)) { 
                        await _userManager.AddToRoleAsync(newUser, role);
                    }
                }
            }

            if (!createRes.Succeeded)
                return Json(new {success = false, message = createRes.ToString()});

            await _userManager.AddToRoleAsync(newUser, AkRoles.Medlem);

            return Json(new {success = true, message = "Användare skapades"});
        }

        [Route("RemoveUser")]
        public async Task<ActionResult> RemoveUser(string userName)
        {
            var res = await _userManager.FindByNameAsync(userName);
            var delRes = await _userManager.DeleteAsync(res);
            if (delRes.Succeeded)
                return Json(new {success = true, message = "Användare borttagen"});
            return Json(new {success = false, message = delRes.ToString()});
        }

        [Route("AddRole")]
        public async Task<ActionResult> AddRole(string UserName, string Role)
        {
            var user = await _userManager.FindByNameAsync(UserName);
            var role = await _roleManager.FindByNameAsync(Role);
            if ((user == null) || (role == null))
                return Json(new {success = false, message = "Misslyckades att lägga till roll"});

            var result = await _userManager.AddToRoleAsync(user, Role);
            if (result.Succeeded)
                return Json(new {success = true});
            return Json(new {success = false, message = "Misslyckades att lägga till roll"});
        }

        [Route("RemoveRole")]
        public async Task<ActionResult> RemoveRole(string UserName, string Role)
        {
            var user = await _userManager.FindByNameAsync(UserName);
            var role = await _roleManager.FindByNameAsync(Role);
            if ((user == null) || (role == null))
                return Json(new {success = false, message = "Misslyckades att ta bort roll"});

            var result = await _userManager.RemoveFromRoleAsync(user, Role);
            if (result.Succeeded)
                return Json(new {success = true});
            return Json(new {success = false, message = result.ToString()});
        }

        [Route("ChangePassword")]
        public async Task<ActionResult> ChangePassword(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, password);
            if (!result.Succeeded)
                return Json(new {success = result.Succeeded, message = result.ToString()});

            return Json(new {success = result.Succeeded, message = "Lösenord ändrat"});
        }

        [Route("AddPost")]
        public async Task<ActionResult> AddPost(string userName, IList<string> post)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return Json(new {success = false, message = "Misslyckades att lägga till post"});
            var poster = JsonConvert.SerializeObject(post);
            user.SlavPoster = poster;
            var result = await _userManager.UpdateAsync(user);
            return Json(new {success = result.Succeeded, message = result.ToString()});
        }

        [Route("SaveMedal")]
        public async Task<ActionResult> SaveMedal(string userName, string medal)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return Json(new {success = false, message = "Misslyckades att lägga till medalj"});
            user.Medal = medal;
            var result = await _userManager.UpdateAsync(user);

            return Json(new {success = result.Succeeded, message = result.ToString()});
        }
    }
}