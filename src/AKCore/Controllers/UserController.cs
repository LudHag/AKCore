using AKCore.DataModel;
using AKCore.Extensions;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Controllers;

[Route("User")]
[Authorize(Roles = "SuperNintendo")]
public class UserController(
    UserManager<AkUser> userManager,
    RoleManager<IdentityRole> roleManager,
    SignInManager<AkUser> signInManager,
    AKContext db
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
        PopulateModel(model);
        return Json(MapToView(model));
    }

    private static UsersViewModel MapToView(UsersModel model)
    {
        var viewModel = new UsersViewModel
        {
            Users = []
        };
        foreach (var user in model.Users)
        {

            var roles = model.Roles[user.UserName].ToArray();
            viewModel.Users.Add(new AkUserViewModel()
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FirstName + " " + user.LastName,
                Email = user.Email,
                Id = user.Id,
                GivenMedal = user.GivenMedal,
                HasKey = user.HasKey,
                Instrument = user.Instrument,
                Medal = user.Medal,
                OtherInstruments = string.IsNullOrWhiteSpace(user.OtherInstruments) ? [] : user.OtherInstruments.Split(',').ToList(),
                Phone = user.Phone,
                SlavPoster = user.SlavPoster,
                Roles = roles,
                Posts = model.Posts[user.UserName] ?? [],
                Active = roles.Length != 0,
                LastSignedIn = (user.LastSignedIn != DateTime.MinValue) ? user.LastSignedIn.ToString("d") : "",
                FoodPreference = user.FoodPreference
            });
        }

        return viewModel;
    }

    private void PopulateModel(UsersModel model)
    {
        IList<AkUser> users;
        var roles = roleManager.Roles.ToList();

        if (model.Inactive)
        {
            users = userManager.Users.Include(u => u.Roles).ToList();
        }
        else
        {
            users = userManager.Users.Include(u => u.Roles).Where(x => x.Roles.Count > 0).ToList();
            
        }

        if (model.SearchPhrase != null) { 
            users = users.Where(x =>
                            x.UserName.Contains(model.SearchPhrase) ||
                            (x.FirstName + ' ' + x.LastName).Contains(model.SearchPhrase)).ToList();
        }
        foreach (var user in users)
        {
            var uRoles = (from role in user.Roles
                          select roles.FirstOrDefault(x => x.Id == role.RoleId)
                into t
                          where t != null
                          select t.Name).ToList();

            model.Roles[user.UserName] = uRoles;
            model.Posts[user.UserName] = user.SlavPoster is not null and not "[null]"
                ? ParseToArray(user.SlavPoster)
                : [];
        }

        model.Users = users.OrderBy(x => x.FirstName).ToList();
    }

    private static string[] ParseToArray(string input)
    {
        string trimmedInput = input.Trim('[', ']');

        var items = trimmedInput.Split(',')
                                .Select(item => item.Trim().Trim('"')) 
                                .Where(item => !string.IsNullOrWhiteSpace(item))
                                .ToArray();

        return items;
    }

    [HttpPost("EditUser")]
    public async Task<ActionResult> EditUser(ProfileModel model)
    {
        var user = await userManager.FindByIdAsync(model.Id);
        var editingUser = await userManager.FindByNameAsync(User.Identity.Name);

        if (user == null)
        {
            return Json(new { success = false, message = "Användare finns ej" });
        }

        var updateUName = user.UserName != model.UserName;
        user.UserName = model.UserName;
        user.Email = model.Email;
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Phone = model.Phone;
        user.Instrument = model.Instrument;
        user.SlavPoster = model.Posts == null ? "" : JsonConvert.SerializeObject(model.Posts);
        user.OtherInstruments = model.OtherInstruments == null ? "" : string.Join(",", model.OtherInstruments);
        user.Medal = model.Medal;
        user.GivenMedal = model.GivenMedal;
        user.FoodPreference = model.FoodPreference;

        var result = await userManager.UpdateAsync(user);

        if (result.Succeeded && user.Id == editingUser.Id && updateUName)
        {
            await signInManager.SignInAsync(editingUser, true);
        }

        if (result.Succeeded)
        {
            db.Log.Add(new LogItem()
            {
                Type = AkLogTypes.User,
                Modified = DateTime.Now.ConvertToSwedishTime(),
                ModifiedBy = editingUser,
                Comment = "Användare med namn " + user.GetName() + " redigeras"
            });
            db.SaveChanges();
        }
        return Json(new
        {
            success = result.Succeeded,
            message = result.Succeeded ? "Uppdaterade användarinfo" : result.ToString()
        });
    }

    [HttpPost("CreateUser")]
    public async Task<ActionResult> CreateUser(ProfileModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
        {
            return Json(new { success = false, message = "Användarnamn och lösenord krävs" });
        }

        var oldUser = await userManager.FindByNameAsync(model.UserName);
        if (oldUser != null)
        {
            return Json(new { success = false, message = "Användarnamn finns redan" });
        }

        var newUser = new AkUser
        {
            UserName = model.UserName,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Phone = model.Phone,
            Instrument = model.Instrument,
            SlavPoster = model.Posts == null ? "" : JsonConvert.SerializeObject(model.Posts),
            Medal = model.Medal,
            GivenMedal = model.GivenMedal,
            FoodPreference = model.FoodPreference
        };

        var createRes = await userManager.CreateAsync(newUser, model.Password);

        if (!createRes.Succeeded)
        {
            return Json(new { success = false, message = string.Join(" ", createRes.ToString()) });
        }

        if (model.Roles != null)
        {
            foreach (var role in model.Roles)
            {
                if (!string.IsNullOrWhiteSpace(role))
                {
                    await userManager.AddToRoleAsync(newUser, role);
                }
            }
        }

        if (!createRes.Succeeded)
            return Json(new { success = false, message = createRes.ToString() });

        var editingUser = await userManager.FindByNameAsync(User.Identity.Name);
        db.Log.Add(new LogItem()
        {
            Type = AkLogTypes.User,
            Modified = DateTime.Now.ConvertToSwedishTime(),
            ModifiedBy = editingUser,
            Comment = "Användare med namn " + newUser.GetName() + " skapad"
        });
        db.SaveChanges();

        return Json(new { success = true, message = "Användare skapades" });
    }

    [Route("RemoveUser")]
    public async Task<ActionResult> RemoveUser(string userName)
    {
        var res = await userManager.FindByNameAsync(userName);
        var fullName = res.GetName();
        var delRes = await userManager.DeleteAsync(res);

        if (delRes.Succeeded)
        {
            var editingUser = await userManager.FindByNameAsync(User.Identity.Name);
            db.Log.Add(new LogItem()
            {
                Type = AkLogTypes.User,
                Modified = DateTime.Now.ConvertToSwedishTime(),
                ModifiedBy = editingUser,
                Comment = "Användare med namn " + fullName + " borttagen"
            });
            db.SaveChanges();

            return Json(new { success = true, message = "Användare borttagen" });
        }

        return Json(new { success = false, message = delRes.ToString() });
    }

    [Route("AddRole")]
    public async Task<ActionResult> AddRole(string UserName, string Role)
    {
        var user = await userManager.FindByNameAsync(UserName);
        var role = await roleManager.FindByNameAsync(Role);
        if ((user == null) || (role == null))
            return Json(new { success = false, message = "Misslyckades att lägga till roll" });

        var result = await userManager.AddToRoleAsync(user, Role);
        if (!result.Succeeded) return Json(new { success = false, message = "Misslyckades att lägga till roll" });
        var editingUser = await userManager.FindByNameAsync(User.Identity.Name);
        db.Log.Add(new LogItem()
        {
            Type = AkLogTypes.User,
            Modified = DateTime.Now.ConvertToSwedishTime(),
            ModifiedBy = editingUser,
            Comment = "Användare med namn " + UserName + " får roll " + Role + " tillagd"
        });
        db.SaveChanges();
        return Json(new { success = true, message = "Lyckades lägga till roll" });
    }

    [Route("RemoveRole")]
    public async Task<ActionResult> RemoveRole(string UserName, string Role)
    {
        var user = await userManager.FindByNameAsync(UserName);
        var role = await roleManager.FindByNameAsync(Role);
        if ((user == null) || (role == null))
            return Json(new { success = false, message = "Misslyckades att ta bort roll" });

        var result = await userManager.RemoveFromRoleAsync(user, Role);
        if (!result.Succeeded) return Json(new { success = false, message = result.ToString() });
        var editingUser = await userManager.FindByNameAsync(User.Identity.Name);
        db.Log.Add(new LogItem()
        {
            Type = AkLogTypes.User,
            Modified = DateTime.Now.ConvertToSwedishTime(),
            ModifiedBy = editingUser,
            Comment = "Användare med namn " + UserName + " får roll " + Role + " borttagen"
        });
        db.SaveChanges();
        return Json(new { success = true, message = "Lyckades ta bort roll" });
    }

    [Route("ChangePassword")]
    public async Task<ActionResult> ChangePassword(string userName, string password)
    {
        var user = await userManager.FindByNameAsync(userName);

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, password);
        if (!result.Succeeded)
            return Json(new { success = result.Succeeded, message = result.ToString() });

        var editingUser = await userManager.FindByNameAsync(User.Identity.Name);
        db.Log.Add(new LogItem()
        {
            Type = AkLogTypes.User,
            Modified = DateTime.Now.ConvertToSwedishTime(),
            ModifiedBy = editingUser,
            Comment = "Användare med namn " + userName + " får lösenord ändrat"
        });
        db.SaveChanges();

        return Json(new { success = result.Succeeded, message = "Lösenord ändrat" });
    }

    [Route("AddPost")]
    public async Task<ActionResult> AddPost([FromBody] AddPostRequest request)
    {
        var user = await userManager.FindByNameAsync(request.UserName);
        if (user == null)
            return Json(new { success = false, message = "Misslyckades att lägga till post" });
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
            return Json(new { success = false, message = "Misslyckades att lägga till medalj" });
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
            return Json(new { success = false, message = "Misslyckades att lägga till medalj" });
        user.GivenMedal = medal;
        var result = await userManager.UpdateAsync(user);

        return Json(new
        {
            success = result.Succeeded,
            message = result.Succeeded ? "Uppdaterade medaljinfo" : result.ToString()
        });
    }
}