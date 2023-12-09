using AKCore.DataModel;
using AKCore.Extensions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace AKCore.Services;

public class AdminLogService
{

    private readonly AKContext _db;
    private readonly UserManager<AkUser> _userManager;
    public AdminLogService(AKContext db, UserManager<AkUser> userManager)
    {
        _db = db;
        _userManager = userManager;
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

    public async Task LogAction(string logType, string userName, string message)
    {
        var user = await _userManager.FindByNameAsync(userName);

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
