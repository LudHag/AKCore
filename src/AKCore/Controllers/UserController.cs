using AKCore.Models;
using AKCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AKCore.Controllers;

[Route("User")]
[Authorize(Roles = "SuperNintendo")]
public class UserController(UserAdminService userAdminService) : Controller
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
        var result = await userAdminService.AddPostAsync(request, User.Identity.Name);
        return Json(new { success = result.Success, message = result.Message });
    }

    [Route("SaveMedal")]
    public async Task<ActionResult> SaveMedal(string userName, string medal)
    {
        var result = await userAdminService.SaveMedalAsync(userName, medal, User.Identity.Name);
        return Json(new { success = result.Success, message = result.Message });
    }

    [Route("SaveGivenMedal")]
    public async Task<ActionResult> SaveGivenMedal(string userName, string medal)
    {
        var result = await userAdminService.SaveGivenMedalAsync(userName, medal, User.Identity.Name);
        return Json(new { success = result.Success, message = result.Message });
    }
}
