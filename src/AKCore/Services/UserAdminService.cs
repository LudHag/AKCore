using AKCore.DataModel;
using AKCore.Extensions;
using AKCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Services;

public class UserAdminService
{
    private readonly UserManager<AkUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<AkUser> _signInManager;
    private readonly AdminLogService _adminLogService;

    public UserAdminService(
        UserManager<AkUser> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<AkUser> signInManager,
        AdminLogService adminLogService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _adminLogService = adminLogService;
    }

    public void PopulateUsersModel(UsersModel model)
    {
        IList<AkUser> users;
        var roles = _roleManager.Roles.ToList();

        if (model.Inactive)
        {
            users = _userManager.Users.Include(u => u.Roles).ToList();
        }
        else
        {
            users = _userManager.Users.Include(u => u.Roles).Where(x => x.Roles.Count > 0).ToList();
        }

        if (model.SearchPhrase != null)
        {
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
                ? ParseSlavPosterToArray(user.SlavPoster)
                : [];
        }

        model.Users = users.OrderBy(x => x.FirstName).ToList();
    }

    public static UsersViewModel MapToViewModel(UsersModel model)
    {
        var viewModel = new UsersViewModel { Users = [] };
        foreach (var user in model.Users)
        {
            var roles = model.Roles[user.UserName].ToArray();
            viewModel.Users.Add(new AkUserViewModel
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
                OtherInstruments = ParseOtherInstruments(user.OtherInstruments),
                Phone = user.Phone,
                SlavPoster = user.SlavPoster,
                Roles = roles,
                Posts = model.Posts[user.UserName] ?? [],
                Active = roles.Length != 0,
                LastSignedIn = user.LastSignedIn != DateTime.MinValue ? user.LastSignedIn.ToString("d") : ""
            });
        }

        return viewModel;
    }

    public static ProfileModel MapToProfileModel(AkUser user, IList<string> roles, IList<string> posts)
    {
        if (user.SlavPoster == "[null]")
        {
            user.SlavPoster = null;
        }

        return new ProfileModel
        {
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.Phone,
            Instrument = user.Instrument,
            OtherInstruments = ParseOtherInstruments(user.OtherInstruments),
            Posts = posts,
            Roles = roles,
            Medal = user.Medal,
            GivenMedal = user.GivenMedal
        };
    }
  
    public static void ApplyProfileFields(AkUser user, ProfileModel model, bool includeAdminFields)
    {
        user.UserName = model.UserName;
        user.Email = model.Email;
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Phone = model.Phone;
        user.Instrument = model.Instrument;
        user.OtherInstruments = model.OtherInstruments == null ? "" : string.Join(",", model.OtherInstruments);

        if (includeAdminFields)
        {
            user.SlavPoster = model.Posts == null ? "" : JsonConvert.SerializeObject(model.Posts);
            user.Medal = model.Medal;
            user.GivenMedal = model.GivenMedal;
        }
    }

    public async Task<ServiceResult> EditUserAsync(ProfileModel model, string editingUserName)
    {
        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null)
        {
            return ServiceResult.Fail("Användare finns ej");
        }

        var editingUser = await _userManager.FindByNameAsync(editingUserName);
        var updateUName = user.UserName != model.UserName;
        ApplyProfileFields(user, model, includeAdminFields: true);

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded && user.Id == editingUser.Id && updateUName)
        {
            await _signInManager.SignInAsync(editingUser, true);
        }

        if (result.Succeeded)
        {
            await _adminLogService.LogAction(AkLogTypes.User, editingUser,
                "Användare med namn " + user.GetName() + " redigeras");
        }

        return new ServiceResult
        {
            Success = result.Succeeded,
            Message = result.Succeeded ? "Uppdaterade användarinfo" : result.ToString()
        };
    }

    public async Task<ServiceResult> CreateUserAsync(ProfileModel model, string editingUserName)
    {
        if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
        {
            return ServiceResult.Fail("Användarnamn och lösenord krävs");
        }

        var oldUser = await _userManager.FindByNameAsync(model.UserName);
        if (oldUser != null)
        {
            return ServiceResult.Fail("Användarnamn finns redan");
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
            GivenMedal = model.GivenMedal
        };

        var createRes = await _userManager.CreateAsync(newUser, model.Password);
        if (!createRes.Succeeded)
        {
            return ServiceResult.Fail(string.Join(" ", createRes.ToString()));
        }

        if (model.Roles != null)
        {
            foreach (var role in model.Roles)
            {
                if (!string.IsNullOrWhiteSpace(role))
                {
                    await _userManager.AddToRoleAsync(newUser, role);
                }
            }
        }

        await _adminLogService.LogAction(AkLogTypes.User, editingUserName,
            "Användare med namn " + newUser.GetName() + " skapad");
        return ServiceResult.Ok("Användare skapades");
    }

    public async Task<ServiceResult> RemoveUserAsync(string userName, string editingUserName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            return ServiceResult.Fail("Användare finns ej");
        }

        var fullName = user.GetName();
        var delRes = await _userManager.DeleteAsync(user);
        if (!delRes.Succeeded)
        {
            return ServiceResult.Fail(delRes.ToString());
        }

        await _adminLogService.LogAction(AkLogTypes.User, editingUserName,
            "Användare med namn " + fullName + " borttagen");
        return ServiceResult.Ok("Användare borttagen");
    }

    public async Task<ServiceResult> AddRoleAsync(string userName, string roleName, string editingUserName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        var role = await _roleManager.FindByNameAsync(roleName);
        if (user == null || role == null)
        {
            return ServiceResult.Fail("Misslyckades att lägga till roll");
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            return ServiceResult.Fail("Misslyckades att lägga till roll");
        }

        await _adminLogService.LogAction(AkLogTypes.User, editingUserName,
            "Användare med namn " + userName + " får roll " + roleName + " tillagd");
        return ServiceResult.Ok("Lyckades lägga till roll");
    }

    public async Task<ServiceResult> RemoveRoleAsync(string userName, string roleName, string editingUserName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        var role = await _roleManager.FindByNameAsync(roleName);
        if (user == null || role == null)
        {
            return ServiceResult.Fail("Misslyckades att ta bort roll");
        }

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            return ServiceResult.Fail(result.ToString());
        }

        await _adminLogService.LogAction(AkLogTypes.User, editingUserName,
            "Användare med namn " + userName + " får roll " + roleName + " borttagen");
        return ServiceResult.Ok("Lyckades ta bort roll");
    }

    public async Task<ServiceResult> ChangePasswordAsync(string userName, string password, string editingUserName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            return ServiceResult.Fail("Användare finns ej");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, password);
        if (!result.Succeeded)
        {
            return new ServiceResult { Success = false, Message = result.ToString() };
        }

        await _adminLogService.LogAction(AkLogTypes.User, editingUserName,
            "Användare med namn " + userName + " får lösenord ändrat");
        return ServiceResult.Ok("Lösenord ändrat");
    }

    public async Task<ServiceResult> AddPostAsync(AddPostRequest request, string adminUserName)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        if (user == null)
        {
            return ServiceResult.Fail("Misslyckades att lägga till post");
        }

        user.SlavPoster = JsonConvert.SerializeObject(request.Post);
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            await _adminLogService.LogAction(AkLogTypes.User, adminUserName,
                "Användare med namn " + user.GetName() + " får poster uppdaterade");
        }

        return new ServiceResult
        {
            Success = result.Succeeded,
            Message = result.Succeeded ? "Uppdaterat poster" : result.ToString()
        };
    }

    public async Task<ServiceResult> SaveMedalAsync(string userName, string medal, string adminUserName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            return ServiceResult.Fail("Misslyckades att lägga till medalj");
        }

        user.Medal = medal;
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            await _adminLogService.LogAction(AkLogTypes.User, adminUserName,
                "Användare med namn " + user.GetName() + " får medalj uppdaterad");
        }

        return new ServiceResult
        {
            Success = result.Succeeded,
            Message = result.Succeeded ? "Uppdaterade medaljinfo" : result.ToString()
        };
    }

    public async Task<ServiceResult> SaveGivenMedalAsync(string userName, string medal, string adminUserName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            return ServiceResult.Fail("Misslyckades att lägga till medalj");
        }

        user.GivenMedal = medal;
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            await _adminLogService.LogAction(AkLogTypes.User, adminUserName,
                "Användare med namn " + user.GetName() + " får utdelad medalj uppdaterad");
        }

        return new ServiceResult
        {
            Success = result.Succeeded,
            Message = result.Succeeded ? "Uppdaterade medaljinfo" : result.ToString()
        };
    }

    public static IList<string> ParseOtherInstruments(string otherInstruments) =>
        string.IsNullOrWhiteSpace(otherInstruments) ? [] : otherInstruments.Split(',').ToList();

    public static string[] ParseSlavPosterToArray(string input)
    {
        var trimmedInput = input.Trim('[', ']');
        return trimmedInput.Split(',')
            .Select(item => item.Trim().Trim('"'))
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .ToArray();
    }

    public static List<string> ParseSlavPosterToList(string slavPoster)
    {
        if (string.IsNullOrWhiteSpace(slavPoster))
        {
            return [];
        }

        var deserialized = JsonConvert.DeserializeObject<List<string>>(slavPoster);
        if (deserialized == null || deserialized.Count == 0)
        {
            return [];
        }

        return [.. deserialized
            .SelectMany(p => p.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Select(p => p.Trim())
            .Where(p => !string.IsNullOrEmpty(p))];
    }
}
