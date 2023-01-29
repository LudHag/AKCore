using AKCore.DataModel;
using AKCore.Extensions;
using System;
using System.Threading.Tasks;

namespace AKCore.Services;

public class AdminLogService
{

    private readonly AKContext _db;

    public AdminLogService(AKContext db)
    {
        _db = db;
    }
    public async Task LogAction(string logType, AkUser user, string message)
    {
        await _db.Log.AddAsync(new LogItem()
        {
            Type = logType,
            Modified = DateTime.Now.ConvertToSwedishTime(),
            ModifiedBy = user,
            Comment = message
        });
        await _db.SaveChangesAsync();
    }
}
