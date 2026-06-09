using AKCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace AKCore.Extensions;

public static class ControllerExtensions
{
    public static JsonResult ServiceJson(this Controller controller, ServiceResult result) =>
        controller.Json(new { success = result.Success, message = result.Message });
}
