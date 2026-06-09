using AKCore.DataModel;
using AKCore.Models;
using AKCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Controllers;

[Route("upcoming")]
[Route("Gigs")]
public class UpcomingController : Controller
{
    private readonly UserManager<AkUser> _userManager;
    private readonly TranslationsService _translationsService;
    private readonly EventService _eventService;
    private readonly SignupService _signupService;

    public UpcomingController(
        UserManager<AkUser> userManager,
        TranslationsService translationsService,
        EventService eventService,
        SignupService signupService)
    {
        _userManager = userManager;
        _translationsService = translationsService;
        _eventService = eventService;
        _signupService = signupService;
    }

    public ActionResult Index()
    {
        ViewBag.Title = _translationsService.Get(TranslationDomains.Upcoming, "PageTitle");
        ViewData["Canonical"] = "https://www.altekamereren.org/upcoming";
        return View();
    }

    [Route("UpcomingListData")]
    public async Task<ActionResult> UpcomingData()
    {
        var loggedIn = User.Identity.IsAuthenticated;
        var member = false;
        var userId = "";
        if (loggedIn)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var roles = await _userManager.GetRolesAsync(user);
            member = roles.Contains("Medlem");
            userId = user.Id;
        }

        var model = _eventService.BuildUpcomingViewModel(loggedIn, member, userId, _translationsService.IsEnglish());
        model.IcalLink = $"{Request.Scheme}://{Request.Host}/upcoming/akevents.ics";
        return Json(model);
    }

    [Route("akevents.ics")]
    public ActionResult Ical(string rehearsalFilter)
    {
        var bytes = _eventService.GenerateIcal(rehearsalFilter);
        return File(bytes, "application/octet-stream", "akevents.ics");
    }

    [Route("EditSignup")]
    [Authorize(Roles = AkRoles.SuperNintendo)]
    [HttpPost]
    public async Task<ActionResult> EditSignup(string eventId, string memberId, string type, bool instrument, bool car)
    {
        if (!int.TryParse(eventId, out var eIdInt))
        {
            return Json(new { success = false, message = _translationsService.Get(TranslationDomains.Upcoming, "InvalidData") });
        }

        try
        {
            await _signupService.EditSignupAsync(eIdInt, memberId, type, instrument, car);
            return Json(new { success = true });
        }
        catch (AkValidationError)
        {
            return Json(new { success = false, message = _translationsService.Get(TranslationDomains.Upcoming, "InvalidData") });
        }
    }

    [Route("Event/{id:int}")]
    [Authorize(Roles = "Medlem")]
    public ActionResult Event(string id)
    {
        ViewBag.Title = _translationsService.Get(TranslationDomains.Upcoming, "SignupPageTitle");
        if (!int.TryParse(id, out _))
        {
            return Redirect("/upcoming");
        }

        return View(id);
    }

    [Route("Event/EventData/{id:int}")]
    [Authorize(Roles = "Medlem")]
    public async Task<ActionResult> EventData(string id)
    {
        if (!int.TryParse(id, out var eId))
        {
            return Json(false);
        }

        var spelning = _signupService.GetEventWithSignups(eId);
        if (spelning == null)
        {
            return Json(false);
        }

        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        var roles = await _userManager.GetRolesAsync(user);
        var nintendo = roles.Contains("SuperNintendo");
        var model = await _signupService.BuildSignUpModelAsync(spelning, user, nintendo, _translationsService.IsEnglish());
        return Json(model);
    }

    [Route("Signup/{id:int}")]
    [Authorize(Roles = "Medlem")]
    public async Task<ActionResult> SignUp(SignUpModel model, string id)
    {
        if (!int.TryParse(id, out var eId))
        {
            return Json(new { success = false, message = _translationsService.Get(TranslationDomains.Upcoming, "InvalidId") });
        }

        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        try
        {
            await _signupService.SaveSignupAsync(model, eId, user);
            return Json(new { success = true, message = _translationsService.Get(TranslationDomains.Upcoming, "SignupUpdated") });
        }
        catch (AkValidationError error)
        {
            return Json(new
            {
                success = false,
                message = _translationsService.Get(TranslationDomains.Upcoming, error.Message)
            });
        }
    }
}
