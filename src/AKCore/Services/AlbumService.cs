using AKCore.DataModel;
using AKCore.Extensions;
using AKCore.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Services;

public class AlbumService
{

    private readonly AKContext _db;
    private readonly UserManager<AkUser> _userManager;
    private readonly AdminLogService _adminLogService;
    private readonly IWebHostEnvironment _hostingEnv;

    public AlbumService(AKContext db, UserManager<AkUser> userManager, AdminLogService adminLogService, IWebHostEnvironment hostingEnv)
    {
        _db = db;
        _userManager = userManager;
        _adminLogService = adminLogService;
        _hostingEnv = hostingEnv;
    }

    public async Task<IEnumerable<Album>> GetAllAlbums()
    {
        return await _db.Albums.Include(x => x.Tracks).OrderByDescending(x => x.Created).ToListAsync();
    }

    public async Task<Album> AddAlbum(string name, string userName)
    {
        if (_db.Albums.Any(x => x.Name == name))
        {
            throw new AkValidationError("Ett album med det namnet finns redan");
        }

        var album = new Album
        {
            Name = name,
            Created = DateTime.Now.ConvertToSwedishTime()
        };
        _db.Albums.Add(album);
        await _db.SaveChangesAsync();

        var filepath = _hostingEnv.WebRootPath + $@"\albums\" + album.Id + @"\";
        Directory.CreateDirectory(filepath);

        var user = await _userManager.FindByNameAsync(userName);

        await _adminLogService.LogAction(AkLogTypes.Album, user, "Album med namn " + album.Name + " tillagt");

        return album;
    }

    public async Task DeleteAlbum(int albumId, string userName)
    {

        var existingAlbum = await _db.Albums.Include(x => x.Tracks).FirstOrDefaultAsync(x => x.Id == albumId);
        if (existingAlbum == null)
        {
            throw new AkValidationError("Misslyckades med att ta bort album");
        }
        foreach (var track in existingAlbum.Tracks)
        {
            _db.Tracks.Remove(track);
        }

        var albumName = existingAlbum.Name;
        _db.Albums.Remove(existingAlbum);
        await _db.SaveChangesAsync();

        var user = await _userManager.FindByNameAsync(userName);

        await _adminLogService.LogAction(AkLogTypes.Album, user, "Album med namn " + albumName + " borttaget");

        var filepath = _hostingEnv.WebRootPath + $@"\albums\" + albumId + @"\";
        Directory.Delete(filepath, true);
    }

    public async Task UpdateImage(int albumId, string src, string userName)
    {
        var album = await _db.Albums.FirstOrDefaultAsync(x => x.Id == albumId);
        if (album == null)
        {
            throw new AkValidationError("Misslyckades med att ändra albumbild");
        }
        album.Image = src;
        await _db.SaveChangesAsync();

        var user = await _userManager.FindByNameAsync(userName);

        await _adminLogService.LogAction(AkLogTypes.Album, user, "Album med id " + albumId + " uppdaterar bild");

    }

    public async Task ChangeName(int albumId, string name, string userName)
    {
        var album = await _db.Albums.FirstOrDefaultAsync(x => x.Id == albumId);
        if (album == null)
        {
            throw new AkValidationError("Misslyckades med att ändra albumnamn");
        }
        album.Name = name;
        await _db.SaveChangesAsync();

        var user = await _userManager.FindByNameAsync(userName);

        await _adminLogService.LogAction(AkLogTypes.Album, user, "Album med id " + albumId + " uppdaterar namn");
    }

}
