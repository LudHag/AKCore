using AKCore.DataModel;
using AKCore.Extensions;
using AKCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AKCore.Controllers;

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
    public async Task<ActionResult> Login(LoginModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, true, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                user.LastSignedIn = DateTime.Now.ConvertToSwedishTime();
                await _userManager.UpdateAsync(user);
                return Json(new { success = true });
            }
            if (result.IsLockedOut)
            {
                return Json(new { success = false, message = "Utlåst" });
            }

        }
        return Json(new { success = false, message = "Inloggning misslyckades" });
    }
    [Route("Logout")]
    public async Task<ActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Redirect("/");
    }

#if DEBUG

    [Route("InitNintendo")]
    public async Task<ActionResult> InitNintendo()
    {

        foreach (var r in AkRoles.Roles)
        {
            var roleresult = await _roleManager.FindByNameAsync(r);
            if (roleresult != null) continue;
            var role = new IdentityRole(r);
            await _roleManager.CreateAsync(role);
        }

        var newUser = new AkUser() { UserName = "nintendo" };
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

            return Json(new { success = true });
        }

        return Json(new { success = true });
    }
#endif
}