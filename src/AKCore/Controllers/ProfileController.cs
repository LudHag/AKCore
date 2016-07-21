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
    [Route("Profile")]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<AkUser> _userManager;
        private readonly SignInManager<AkUser> _signInManager;

        public ProfileController(
            UserManager<AkUser> userManager,
            SignInManager<AkUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.Title = "Profil";

            var model = new ProfileModel
            {
                UserName = user.UserName,
                Email = user.Email,
                Roles = await _userManager.GetRolesAsync(user)
            };
            return View(model);
        }
        [Route("EditProfile")]
        public async Task<ActionResult> EditProfile(ProfileModel model)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var updateUName = user.UserName != model.UserName;

            user.UserName = model.UserName;
            user.Email = model.Email;
            var result=await _userManager.UpdateAsync(user);
            if (result.Succeeded && updateUName)
            {
                await _signInManager.SignInAsync(user,true);
            }
            return Json(new { success = result.Succeeded , message=result.ToString()});
        }
    }
}