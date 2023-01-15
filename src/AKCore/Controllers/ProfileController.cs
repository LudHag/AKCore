﻿using System.Collections.Generic;
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
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly SignInManager<AkUser> _signInManager;
        private readonly UserManager<AkUser> _userManager;
        private readonly AKContext _db;

        public ProfileController(
            UserManager<AkUser> userManager,
            SignInManager<AkUser> signInManager,
            AKContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Profil";
            return View();
        }

        [Route("ProfileData")]
        public async Task<ActionResult> ProfileData()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var logins = _db.UserLogins.Where(x => x.ProviderDisplayName == user.UserName).ToList();
            if(user.SlavPoster == "[null]")
            {
                user.SlavPoster = null;
            }

            var model = new ProfileModel
            {
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Instrument = user.Instrument,
                OtherInstruments = string.IsNullOrWhiteSpace(user.OtherInstruments) ? null : user.OtherInstruments.Split(',').ToList(),
                Posts = !string.IsNullOrWhiteSpace(user.SlavPoster) ? JsonConvert.DeserializeObject<List<string>>(user.SlavPoster) : new List<string>(),
                Roles = await _userManager.GetRolesAsync(user),
                Medal = user.Medal,
                GivenMedal = user.GivenMedal
            };
            return Json(model);
        }

        [Route("EditProfile")]
        public async Task<ActionResult> EditProfile([FromBody] ProfileModel model)
        {
            if (model.OtherInstruments?.Contains(model.Instrument) ?? false)
            {
                return Json(new { success = false, message = "Du kan inte välja samma instrument som både huvudinstrument och andrainstrument." });
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var updateUName = user.UserName != model.UserName;
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Phone = model.Phone;
            user.Instrument = model.Instrument;
            user.OtherInstruments = model.OtherInstruments == null ? "" : string.Join(",",model.OtherInstruments);
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded && updateUName)
            {
                await _signInManager.SignInAsync(user, true);
            }
            return Json(new {success = result.Succeeded, message = "Din profil uppdaterades"});
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