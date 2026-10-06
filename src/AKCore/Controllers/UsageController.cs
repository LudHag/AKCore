using AKCore.DataModel;
using AKCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AKCore.Controllers;

[Route("Usage")]
[AllowAnonymous]
public class UsageController(UsageCollector usageCollector) : Controller
{
    [HttpPost]
    [Route("Record")]
    public ActionResult Record(string type)
    {
        if (string.IsNullOrWhiteSpace(type) || !AkFeatureUsageTypes.Types.Contains(type))
        {
            return BadRequest();
        }

        usageCollector.Record(type);
        return Json(new { success = true });
    }
}
