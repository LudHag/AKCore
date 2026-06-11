using AKCore.DataModel;
using Microsoft.EntityFrameworkCore;

namespace AKCore.IntegrationTests.TestData;

public static class LogAssertions
{
    public static async Task AssertLogExistsAsync(
        AKContext db,
        string type,
        string commentSubstring,
        string? modifiedByUserName = TestUsers.AdminUserName)
    {
        var logs = await db.Log
            .Include(l => l.ModifiedBy)
            .Where(l => l.Type == type && l.Comment.Contains(commentSubstring))
            .ToListAsync();

        Assert.NotEmpty(logs);

        if (modifiedByUserName != null)
        {
            Assert.Contains(logs, l => l.ModifiedBy?.UserName == modifiedByUserName);
        }
    }

    public static async Task AssertNoLogAsync(AKContext db, string type, string commentSubstring)
    {
        var exists = await db.Log.AnyAsync(l => l.Type == type && l.Comment.Contains(commentSubstring));
        Assert.False(exists);
    }
}
