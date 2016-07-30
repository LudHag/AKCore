using System.Threading.Tasks;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AKCore.Controllers
{
    [Route("Profile")]
    [Authorize(Roles = "Medlem")]
    public class ProfileController : Controller
    {
        private readonly SignInManager<AkUser> _signInManager;
        private readonly UserManager<AkUser> _userManager;

        public ProfileController(
            UserManager<AkUser> userManager,
            SignInManager<AkUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.Title = "Profil";

            var model = new ProfileModel
            {
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Adress = user.Adress,
                ZipCode = user.ZipCode,
                City = user.City,
                Phone = user.Phone,
                Nation = user.Nation,
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
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Adress = model.Adress;
            user.ZipCode = model.ZipCode;
            user.City = model.City;
            user.Phone = model.Phone;
            user.Nation = model.Nation;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded && updateUName)
            {
                await _signInManager.SignInAsync(user, true);
            }
            return Json(new {success = result.Succeeded, message = result.ToString()});
        }

        [Route("ChangePassword")]
        public async Task<ActionResult> ChangePassword(string password)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, password);
            if (!result.Succeeded)
            {
                return Json(new {success = result.Succeeded, message = result.ToString()});
            }

            return Json(new {success = result.Succeeded, message = "Lösenord ändrat"});
        }
    }
}