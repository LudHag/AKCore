using AKCore.DataModel;
using AKCore.Models;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Services;

public class RecruitService
{
    private readonly AKContext _db;
    private readonly AdminLogService _adminLogService;

    public RecruitService(AKContext db, AdminLogService adminLogService)
    {
        _db = db;
        _adminLogService = adminLogService;
    }

    public async Task<ServiceResult> ArchiveAsync(int id, bool arch, string adminUserName)
    {
        if (id < 0)
        {
            return ServiceResult.Fail("Ogiltigt id");
        }

        var recruit = _db.Recruits.FirstOrDefault(x => x.Id == id);
        if (recruit == null)
        {
            return ServiceResult.Fail("Anmälan finns ej");
        }

        recruit.Archived = arch;
        await _db.SaveChangesAsync();
        await _adminLogService.LogAction(AkLogTypes.Recruits, adminUserName,
            "Anmälan med id " + id + (arch ? " arkiverad" : " avarkiverad"));

        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> RemoveAsync(int id, string adminUserName)
    {
        if (id < 0)
        {
            return ServiceResult.Fail("Ogiltigt id");
        }

        var recruit = _db.Recruits.FirstOrDefault(x => x.Id == id);
        if (recruit == null)
        {
            return ServiceResult.Fail("Anmälan finns ej");
        }

        _db.Recruits.Remove(recruit);
        await _db.SaveChangesAsync();
        await _adminLogService.LogAction(AkLogTypes.Recruits, adminUserName,
            "Anmälan med id " + id + " borttagen");

        return ServiceResult.Ok();
    }
}
