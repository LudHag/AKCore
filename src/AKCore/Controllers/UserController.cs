using AKCore.DataModel;
using AKCore.Models;
using AKCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace AKCore.Controllers;

[Route("User")]
[Authorize(Roles = "SuperNintendo")]
public class UserController(
    UserManager<AkUser> userManager,
    UserAdminService userAdminService
    ) : Controller
{
    public ActionResult Index()
    {
        ViewBag.Title = "Användare";
        return View();
    }

    [Route("UserListData")]
    public ActionResult UserListData(UsersModel model)
    {
        userAdminService.PopulateUsersModel(model);
        return Json(UserAdminService.MapToViewModel(model));
    }

    [HttpPost("EditUser")]
    public async Task<ActionResult> EditUser(ProfileModel model)
    {
        var result = await userAdminService.EditUserAsync(model, User.Identity.Name);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost("CreateUser")]
    public async Task<ActionResult> CreateUser(ProfileModel model)
    {
        var result = await userAdminService.CreateUserAsync(model, User.Identity.Name);
        return Json(new { success = result.Success, message = result.Message });
    }

    [Route("RemoveUser")]
    public async Task<ActionResult> RemoveUser(string userName)
    {
        var result = await userAdminService.RemoveUserAsync(userName, User.Identity.Name);
        return Json(new { success = result.Success, message = result.Message });
    }

    [Route("AddRole")]
    public async Task<ActionResult> AddRole(string UserName, string Role)
    {
        var result = await userAdminService.AddRoleAsync(UserName, Role, User.Identity.Name);
        return Json(new { success = result.Success, message = result.Message });
    }

    [Route("RemoveRole")]
    public async Task<ActionResult> RemoveRole(string UserName, string Role)
    {
        var result = await userAdminService.RemoveRoleAsync(UserName, Role, User.Identity.Name);
        return Json(new { success = result.Success, message = result.Message });
    }

    [Route("ChangePassword")]
    public async Task<ActionResult> ChangePassword(string userName, string password)
    {
        var result = await userAdminService.ChangePasswordAsync(userName, password, User.Identity.Name);
        return Json(new { success = result.Success, message = result.Message });
    }

    [Route("AddPost")]
    public async Task<ActionResult> AddPost([FromBody] AddPostRequest request)
    {
        var user = await userManager.FindByNameAsync(request.UserName);
        if (user == null)
        {
            return Json(new { success = false, message = "Misslyckades att lägga till post" });
        }

        var poster = JsonConvert.SerializeObject(request.Post);
        user.SlavPoster = poster;
        var result = await userManager.UpdateAsync(user);
        return Json(new
        {
            success = result.Succeeded,
            message = result.Succeeded ? "Uppdaterat poster" : result.ToString()
        });
    }

    [Route("SaveMedal")]
    public async Task<ActionResult> SaveMedal(string userName, string medal)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user == null)
        {
            return Json(new { success = false, message = "Misslyckades att lägga till medalj" });
        }

        user.Medal = medal;
        var result = await userManager.UpdateAsync(user);
        return Json(new
        {
            success = result.Succeeded,
            message = result.Succeeded ? "Uppdaterade medaljinfo" : result.ToString()
        });
    }

    [Route("SaveGivenMedal")]
    public async Task<ActionResult> SaveGivenMedal(string userName, string medal)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user == null)
        {
            return Json(new { success = false, message = "Misslyckades att lägga till medalj" });
        }

        user.GivenMedal = medal;
        var result = await userManager.UpdateAsync(user);
        return Json(new
        {
            success = result.Succeeded,
            message = result.Succeeded ? "Uppdaterade medaljinfo" : result.ToString()
        });
    }
}
