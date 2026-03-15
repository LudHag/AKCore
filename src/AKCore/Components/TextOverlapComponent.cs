using AKCore.DataModel;
using Microsoft.AspNetCore.Mvc;

namespace AKCore.Components;

public class TextOverlapViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(Widget widget)
    {
        return View(widget);
    }
}
