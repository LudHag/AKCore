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

        [Route("InitNintendo")]
        public async System.Threading.Tasks.Task<ActionResult> InitNintendo()
        {

            var roleresult = await _roleManager.FindByNameAsync("SuperNintendo");
            var nintendoRole = new IdentityRole("SuperNintendo");

            if (roleresult == null)
            {
                await _roleManager.CreateAsync(nintendoRole);
            }

            var roleresult2 = await _roleManager.FindByNameAsync("Medlem");
            var memberRole = new IdentityRole("Medlem");

            if (roleresult2 == null)
            {
                await _roleManager.CreateAsync(memberRole);
            }

            var newUser = new AkUser() {UserName = "nintendo"};
            var newCommonUser = new AkUser() { UserName = "test" };

            var user = await _userManager.FindByNameAsync("nintendo");
            if (user == null)
            {
                await _userManager.CreateAsync(newUser, "123456");
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "SuperNintendo");
                await _userManager.AddToRoleAsync(user, "Medlem");
            }
            
            var user2 = await _userManager.FindByNameAsync("test");
            if (user2 == null)
            {
                await _userManager.CreateAsync(newCommonUser, "123456");
                return Json(new {success = true});
            }
            else if(user2.Roles.Count<1)
            {
                var res= await _userManager.AddToRoleAsync(user2, "Medlem");
            }


            return Json(new {success = true});
        }
    }
}