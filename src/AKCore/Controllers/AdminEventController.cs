using AKCore.Models;
using AKCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace AKCore.Controllers;

[Route("AdminEvent")]
[Authorize(Roles = "SuperNintendo")]
public class AdminEventController : Controller
{
    private readonly EventService _eventService;

    public AdminEventController(EventService eventService)
    {
        _eventService = eventService;
    }

    public ActionResult Index()
    {
        ViewBag.Title = "Ändra händelser";
        return View();
    }

    [Route("EventData")]
    public ActionResult EventData(bool old, int page) =>
        Json(_eventService.GetAdminEventList(old, page));

    [HttpPost]
    [Route("Edit")]
    public async Task<ActionResult> Edit(EventViewModel model)
    {
        var result = await _eventService.SaveEventAsync(model, User.Identity.Name);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost]
    [Route("Remove/{id:int}")]
    public async Task<ActionResult> Remove(string id)
    {
        if (!int.TryParse(id, out var eId))
        {
            return Json(new { success = false, message = "Misslyckades med att ta bort event" });
        }

        var result = await _eventService.RemoveEventAsync(eId, User.Identity.Name);
        return Json(new { success = result.Success, message = result.Message });
    }

    [Route("GetEvent/{id:int}")]
    public ActionResult GetEvent(string id)
    {
        if (!int.TryParse(id, out var eId))
        {
            return Json(new { success = false, message = "Misslyckades med att hämta event" });
        }

        var e = _eventService.GetEventById(eId);
        if (e == null)
        {
            return Json(new { success = false, message = "Misslyckades med att hämta event" });
        }

        return Json(new { success = true, e = JsonConvert.SerializeObject(e) });
    }
}
