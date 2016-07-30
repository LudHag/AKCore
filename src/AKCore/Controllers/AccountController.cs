using System.Threading.Tasks;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace AKCore.Controllers
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly SignInManager<AkUser> _signInManager;
        private readonly UserManager<AkUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<AkUser> userManager,
            SignInManager<AkUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        [Route("Login")]
        public async System.Threading.Tasks.Task<ActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, true, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return Json(new { success = true});
                }
                if (result.IsLockedOut)
                {
                    return Json(new { success = false, message = "Utlåst" });
                }
                
            }
            return Json(new { success = false, message = "Inloggning misslyckades" });
        }
        [Route("Logout")]
        public async System.Threading.Tasks.Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }

        [Route("FbLogin")]
        public async Task<ActionResult> FbLogin(string fbId)
        {
            if (fbId == null)
            {
                return Json(new { success = false, message = "Inget login skickat" });
            }

            var res = await _signInManager.ExternalLoginSignInAsync("Facebook", fbId, true);

            return Json(new { success = res.Succeeded, message = res.ToString() });
        }

        [Route("InitNintendo")]
        public async System.Threading.Tasks.Task<ActionResult> InitNintendo()
        {

            var roleresult = await _roleManager.FindByNameAsync(AkRoles.SuperNintendo);
            var nintendoRole = new IdentityRole(AkRoles.SuperNintendo);

            if (roleresult == null)
            {
                await _roleManager.CreateAsync(nintendoRole);
            }

            var roleresult2 = await _roleManager.FindByNameAsync(AkRoles.Medlem);
            var memberRole = new IdentityRole(AkRoles.Medlem);

            if (roleresult2 == null)
            {
                await _roleManager.CreateAsync(memberRole);
            }

            var roleresult3 = await _roleManager.FindByNameAsync(AkRoles.Editor);
            var editorRole = new IdentityRole(AkRoles.Editor);

            if (roleresult3 == null)
            {
                await _roleManager.CreateAsync(editorRole);
            }

            var newUser = new AkUser() {UserName = "nintendo"};
            var newCommonUser = new AkUser() { UserName = "test" };

            var user = await _userManager.FindByNameAsync("nintendo");
            if (user == null)
            {
                await _userManager.CreateAsync(newUser, "123456");
                user = await _userManager.FindByNameAsync("nintendo");
                await _userManager.AddToRoleAsync(user, AkRoles.SuperNintendo);
                await _userManager.AddToRoleAsync(user, AkRoles.Medlem);
            }
            
            var user2 = await _userManager.FindByNameAsync("test");
            if (user2 == null)
            {
                await _userManager.CreateAsync(newCommonUser, "123456");
                user2 = await _userManager.FindByNameAsync("test");
                await _userManager.AddToRoleAsync(user2, AkRoles.Medlem);

                return Json(new {success = true});
            }

            return Json(new {success = true});
        }
    }
}