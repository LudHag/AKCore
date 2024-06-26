using AKCore.DataModel;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text;
using AKCore.Services;

namespace AKCore.Controllers;

public class SitemapController : Controller
{
    private readonly SitemapService _sitemapService;

    public SitemapController(SitemapService sitemapService)
    {
        _sitemapService = sitemapService;
    }

    [HttpGet("sitemap.xml")]
    [Produces("application/xml")]
    public async Task<IActionResult> Get()
    {
        var sitemap = await _sitemapService.GetSiteMap();
        return Content(sitemap, "application/xml", Encoding.UTF8);
    }

    

}