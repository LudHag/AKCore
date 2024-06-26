using AKCore.DataModel;
using AKCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AKCore.Services;

public class SitemapService
{
    private readonly AKContext _db;
    private readonly IMemoryCache _memoryCache;

    public SitemapService(AKContext db, IMemoryCache memoryCache)
    {
        _db = db;
        _memoryCache = memoryCache;
    }

    public async Task<string> GetSiteMap()
    {
        var cacheKey = "sitemap";
        return await _memoryCache.GetOrCreateAsync(cacheKey, cacheEntry =>
        {
            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            return GenerateSitemap();
        });
    }
    private async Task<string> GenerateSitemap()
    {
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        var root = new XElement(ns + "urlset");

        var sitemapItems = await GetSitemapModels();

        foreach (var sitemapItem in sitemapItems)
        {
            var urlElement = new XElement(ns + "url",
                new XElement(ns + "loc", sitemapItem.Url),
                new XElement(ns + "priority", sitemapItem.Priority),
                new XElement(ns + "lastmod", sitemapItem.LastModified)
                );
            root.Add(urlElement);
        }

        var document = new XDocument(root);

        using var memoryStream = new MemoryStream();
        document.Save(memoryStream);
        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }

    private async Task<IEnumerable<SitemapModel>> GetSitemapModels()
    {
        var publicPages = await _db.Pages.Where(page=>!page.LoggedIn).ToListAsync();
        var latestModifiedTime = await _db.Log.Where(x=>x.Type == "Events").OrderByDescending(x => x.Id).Select(x => x.Modified).FirstOrDefaultAsync();

        var pages = publicPages.Select(x => new SitemapModel("https://www.altekamereren.org" + x.Slug, x.LastModified.ToString("yyyy-MM-dd"), "0.8")).ToList();
        pages.Add(new SitemapModel("https://www.altekamereren.org/upcoming", latestModifiedTime.ToString("yyyy-MM-dd"), "1.0"));

        return pages;
    }
}



