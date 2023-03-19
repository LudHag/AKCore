using AKCore.DataModel;
using AKCore.Extensions;
using AKCore.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Services;

public class AlbumService
{
    private static readonly string[] MusicExtensions = { "mp3" };
    private readonly AKContext _db;
    private readonly AdminLogService _adminLogService;
    private readonly IWebHostEnvironment _hostingEnv;

    public AlbumService(AKContext db, AdminLogService adminLogService, IWebHostEnvironment hostingEnv)
    {
        _db = db;
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

        await _adminLogService.LogAction(AkLogTypes.Album, userName, "Album med namn " + album.Name + " tillagt");

        return album;
    }

    public async Task<Album> GetAlbum(int albumId)
    {
        var album = await _db.Albums.Include(x => x.Tracks).FirstOrDefaultAsync(x => x.Id == albumId);
        if (album == null)
        {
            throw new AkValidationError("Album doesnt exist");
        }
        return album;
    }

    public async Task DeleteAlbum(int albumId, string userName)
    {

        var existingAlbum = await GetAlbum(albumId);
        foreach (var track in existingAlbum.Tracks)
        {
            _db.Tracks.Remove(track);
        }

        var albumName = existingAlbum.Name;
        _db.Albums.Remove(existingAlbum);
        await _db.SaveChangesAsync();

        await _adminLogService.LogAction(AkLogTypes.Album, userName, "Album med namn " + albumName + " borttaget");

        var filepath = _hostingEnv.WebRootPath + $@"\albums\" + albumId + @"\";
        Directory.Delete(filepath, true);
    }

    public async Task UpdateImage(int albumId, string src, string userName)
    {
        var album = await GetAlbum(albumId);
        album.Image = src;
        await _db.SaveChangesAsync();

        await _adminLogService.LogAction(AkLogTypes.Album, userName, "Album med id " + albumId + " uppdaterar bild");

    }

    public async Task ChangeName(int albumId, string name, string userName)
    {
        var album = await GetAlbum(albumId);
        album.Name = name;
        await _db.SaveChangesAsync();

        await _adminLogService.LogAction(AkLogTypes.Album, userName, "Album med id " + albumId + " uppdaterar namn");
    }

    public async Task ChangeCategory(int albumId, string category, string userName)
    {
        var album = await GetAlbum(albumId);
        album.Category = category;
        await _db.SaveChangesAsync();

        await _adminLogService.LogAction(AkLogTypes.Album, userName, "Album med id " + albumId + " uppdaterar kategori");
    }


    public async Task<IEnumerable<Track>> UploadTracks(AlbumEditModel model, string userName)
    {
        var files = model.TrackFiles;
        var album = await GetAlbum(model.AlbumId);

        foreach (var file in files)
        {
            var filename = ContentDispositionHeaderValue
                .Parse(file.ContentDisposition)
                .FileName
                .ToString()
                .Trim('"');
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (MusicExtensions.FirstOrDefault(x => ext.EndsWith(x)) == null)
            {
                throw new AkValidationError("Filen/erna har inte formatet mp3");
            }

            if (album.Tracks?.FirstOrDefault(x => x.FileName == filename) != null)
            {
                throw new AkValidationError("Filen finns redan uppladdad");
            }
        }

        foreach (var file in files)
        {
            var filename = ContentDispositionHeaderValue
                .Parse(file.ContentDisposition)
                .FileName
                .ToString()
                .Trim('"');
            var filepath = _hostingEnv.WebRootPath + $@"\albums\{model.AlbumId}\{filename}";

            using (var fs = File.Create(filepath))
            {
                file.CopyTo(fs);
                fs.Flush();
            }
            var track = new Track
            {
                FileName = filename,
                Created = DateTime.Now.ConvertToSwedishTime()
            };
            album.Tracks?.Add(track);
        }
        var n = 1;
        foreach (var t in album.Tracks.OrderBy(x => x.FileName))
        {
            t.Number = n;
            n++;
        }

        await _db.SaveChangesAsync();

        await _adminLogService.LogAction(AkLogTypes.Album, userName, "Album med id " + model.AlbumId + " laddar upp filer");

        return album.Tracks.ToList();
    }

    public async Task ChangeTrackName(int trackId, string name, string userName)
    {
        var track = _db.Tracks.FirstOrDefault(x => x.Id == trackId);
        if (track == null)
        {
            throw new AkValidationError("Misslyckades med att ändra namn på spår");
        }
        track.Name = name;
        await _db.SaveChangesAsync();
        await _adminLogService.LogAction(AkLogTypes.Album, userName, "Musikspår med id " + trackId + " byter namn");
    }


    public async Task<IEnumerable<Track>> DeleteTrack(int trackId, int albumId, string userName)
    {
        var album = await GetAlbum(albumId);
        var track = album.Tracks.FirstOrDefault(x => x.Id == trackId);
        if (track == null)
        {
            throw new AkValidationError("Misslyckades med att ta bort spår");

        }
        var trackName = track.FileName;
        _db.Tracks.Remove(track);
        album.Tracks.Remove(track);
        var n = 1;
        foreach (var t in album.Tracks.OrderBy(x => x.FileName))
        {
            t.Number = n;
            n++;
        }
        await _db.SaveChangesAsync();


        var filepath = _hostingEnv.WebRootPath + $@"\albums\{albumId}\{trackName}";
        File.Delete(filepath);

        await _adminLogService.LogAction(AkLogTypes.Album, userName, "Musikspår med id " + albumId + " tas bort");

        return album.Tracks;
    }
}
