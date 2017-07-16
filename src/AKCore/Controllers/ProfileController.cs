using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;

namespace AKCore.Controllers
{
    [Route("Profile")]
    [Authorize(Roles = "Medlem")]
    public class ProfileController : Controller
    {
        private readonly SignInManager<AkUser> _signInManager;
        private readonly UserManager<AkUser> _userManager;
        private readonly IHostingEnvironment _hostingEnv;

        public ProfileController(
            UserManager<AkUser> userManager,
            SignInManager<AkUser> signInManager,
            IHostingEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _hostingEnv = env;
        }

        public async Task<ActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.Title = "Profil";
            using (var db = new AKContext(_hostingEnv))
            {
                var logins=db.UserLogins.Where(x => x.ProviderDisplayName == user.UserName).ToList();

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
                    Instrument = user.Instrument,
                    OtherInstrument = string.IsNullOrWhiteSpace(user.OtherInstruments) ? null : user.OtherInstruments.Split(',').ToList(),
                    Facebook = logins.Any(x=>x.LoginProvider=="Facebook"),
                    Poster = user.SlavPoster != null ? JsonConvert.DeserializeObject<List<string>>(user.SlavPoster) : new List<string>(),
                    Roles = await _userManager.GetRolesAsync(user),
                    Medal = user.Medal,
                    GivenMedal = user.GivenMedal
                };
                return View(model);
            }
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
            user.Instrument = model.Instrument;
            user.OtherInstruments = model.OtherInstrument == null ? "" : string.Join(",",model.OtherInstrument);
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

        [Route("FbConnect")]
        public async Task<ActionResult> FbConnect(string fbId)
        {
            if (fbId == null)
            {
                return Json(new {success = false, message = "Inget login kopplat"});
            }
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var loginInfo = new UserLoginInfo("Facebook", fbId, user.UserName);
            var addLoginAsync = await _userManager.AddLoginAsync(user, loginInfo);

            return Json(new {success = addLoginAsync.Succeeded, message = addLoginAsync.ToString()});
        }

        [Route("RemoveFbConnect")]
        public async Task<ActionResult> RemoveFbConnect(string fbId)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var res = await _userManager.RemoveLoginAsync(user, "Facebook", fbId);

            if (res.Succeeded)
            {
                await _signInManager.SignInAsync(user, true);
            }

            return Json(new {success = res.Succeeded, message = res.ToString()});
        }
    }
}