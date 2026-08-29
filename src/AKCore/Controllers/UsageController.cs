using AKCore.DataModel;
using AKCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AKCore.Controllers;

[Route("Usage")]
[Authorize]
public class UsageController(UsageCollector usageCollector) : Controller
{
    [HttpPost]
    [Route("Track")]
    public ActionResult Track(string type)
    {
        if (string.IsNullOrWhiteSpace(type) || !AkFeatureUsageTypes.Types.Contains(type))
        {
            return BadRequest();
        }

        usageCollector.Record(type);
        return Json(new { success = true });
    }
}
