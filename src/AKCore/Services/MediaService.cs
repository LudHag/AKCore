using AKCore.DataModel;
using AKCore.Extensions;
using AKCore.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Services;

public class MediaService
{
    private static readonly string[] ImageExtensions = { "jpg", "bmp", "gif", "png", "svg" };
    private static readonly string[] DocumentExtensions = { "pdf", "docx", "doc" };

    private readonly AKContext _db;
    private readonly AdminLogService _adminLogService;
    private readonly IWebHostEnvironment _hostingEnv;

    public MediaService(AKContext db, AdminLogService adminLogService, IWebHostEnvironment hostingEnv)
    {
        _db = db;
        _adminLogService = adminLogService;
        _hostingEnv = hostingEnv;
    }

    public async Task<ServiceResult> EditFileAsync(string tag, string id, string adminUserName)
    {
        if (!int.TryParse(id, out int fileId) || string.IsNullOrWhiteSpace(tag))
        {
            return ServiceResult.Fail("Ogiltig fil");
        }

        var file = _db.Medias.FirstOrDefault(x => x.Id == fileId);
        if (file == null)
        {
            return ServiceResult.Fail("Ogiltig fil");
        }

        file.Tag = tag;
        await _db.SaveChangesAsync();
        await _adminLogService.LogAction(AkLogTypes.Files, adminUserName,
            "Fil med id " + fileId + " får tagg uppdaterad");

        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> UploadFilesAsync(MediasModel model, string adminUserName)
    {
        foreach (var uploadFile in model.UploadFiles)
        {
            var ext = Path.GetExtension(uploadFile.FileName).ToLower();
            var isImage = ImageExtensions.FirstOrDefault(x => ext.EndsWith(x)) != null;
            var isDocument = DocumentExtensions.FirstOrDefault(x => ext.EndsWith(x)) != null;
            if (!(isImage || isDocument) || string.IsNullOrWhiteSpace(model.Tag))
            {
                return ServiceResult.Fail("Filen har fel format");
            }

            var filename = ContentDispositionHeaderValue
                .Parse(uploadFile.ContentDisposition)
                .FileName
                .ToString()
                .Trim('"');
            var filepath = _hostingEnv.WebRootPath + $@"\media\{filename}";

            if (_db.Medias.FirstOrDefault(x => x.Name == filename) != null)
            {
                return ServiceResult.Fail("Filen finns redan uppladdad");
            }

            using (var fs = File.Create(filepath))
            {
                uploadFile.CopyTo(fs);
                fs.Flush();
            }

            var mediaFile = new Media
            {
                Name = filename,
                Created = DateTime.Now.ConvertToSwedishTime(),
                Tag = model.Tag,
                Type = isImage ? "Image" : "Document"
            };
            _db.Medias.Add(mediaFile);
        }

        await _db.SaveChangesAsync();
        await _adminLogService.LogAction(AkLogTypes.Files, adminUserName,
            "Fil(er) uppladdade med tagg " + model.Tag);

        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> RemoveFileAsync(string filename, string adminUserName)
    {
        var filepath = GetSafeMediaFilePath(filename);
        if (filepath == null)
        {
            return ServiceResult.Fail("Ogiltigt filnamn");
        }

        var file = _db.Medias.FirstOrDefault(x => x.Name == filename);
        if (file == null)
        {
            return ServiceResult.Fail("Filen finns ej");
        }

        try
        {
            File.Delete(filepath);
        }
        catch
        {
            return ServiceResult.Fail("Misslyckades ta bort filen");
        }

        _db.Medias.Remove(file);
        await _db.SaveChangesAsync();
        await _adminLogService.LogAction(AkLogTypes.Files, adminUserName,
            "Fil med namn " + filename + " borttagen");

        return ServiceResult.Ok();
    }

    private string? GetSafeMediaFilePath(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename) || filename != Path.GetFileName(filename))
        {
            return null;
        }

        var mediaRoot = Path.GetFullPath(Path.Combine(_hostingEnv.WebRootPath, "media"));
        var filepath = Path.GetFullPath(Path.Combine(mediaRoot, filename));
        if (!filepath.StartsWith(mediaRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return filepath;
    }
}
