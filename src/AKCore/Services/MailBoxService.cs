using AKCore.DataModel;
using AKCore.Models;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Services;

public class MailBoxService
{
    private readonly AKContext _db;
    private readonly AdminLogService _adminLogService;

    public MailBoxService(AKContext db, AdminLogService adminLogService)
    {
        _db = db;
        _adminLogService = adminLogService;
    }

    public async Task<ServiceResult> ArchiveAsync(int id, string adminUserName)
    {
        if (id < 0)
        {
            return ServiceResult.Fail("Ogiltigt id");
        }

        var item = _db.MailBoxItems.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            return ServiceResult.Fail("Meddelande finns ej");
        }

        item.Archived = !item.Archived;
        await _db.SaveChangesAsync();
        await _adminLogService.LogAction(AkLogTypes.MailBox, adminUserName,
            "Meddelande med id " + id + (item.Archived ? " arkiverat" : " avarkiverat"));

        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> RemoveAsync(int id, string adminUserName)
    {
        if (id < 0)
        {
            return ServiceResult.Fail("Ogiltigt id");
        }

        var item = _db.MailBoxItems.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            return ServiceResult.Fail("Meddelande finns ej");
        }

        _db.MailBoxItems.Remove(item);
        await _db.SaveChangesAsync();
        await _adminLogService.LogAction(AkLogTypes.MailBox, adminUserName,
            "Meddelande med id " + id + " borttaget");

        return ServiceResult.Ok();
    }
}
