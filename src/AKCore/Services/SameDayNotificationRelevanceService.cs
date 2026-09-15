using AKCore.DataModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AKCore.Services;

public sealed record SameDayNotificationCandidate(
    int EventId,
    string UserId);

public class SameDayNotificationRelevanceService
{
    private readonly AKContext _db;
    private readonly UserManager<AkUser> _userManager;

    public SameDayNotificationRelevanceService(
        AKContext db,
        UserManager<AkUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IReadOnlyList<SameDayNotificationCandidate>> GetCandidatesAsync(
        DateTime day,
        CancellationToken cancellationToken = default)
    {
        var date = day.Date;
        var nextDate = date.AddDays(1);

        var events = await _db.Events
            .Include(x => x.SignUps)
            .Where(x => x.Day >= date && x.Day < nextDate)
            .ToListAsync(cancellationToken);

        var members = await _userManager.GetUsersInRoleAsync(AkRoles.Medlem);
        var balletMembers = await _userManager.GetUsersInRoleAsync(AkRoles.Balett);
        var balletMemberIds = balletMembers
            .Select(x => x.Id)
            .ToHashSet();

        var candidates = new List<SameDayNotificationCandidate>();

        foreach (var evt in events)
        {
            foreach (var member in members)
            {
                var signup = evt.SignUps?
                    .FirstOrDefault(x => x.PersonId == member.Id);

                if (signup?.Where == AkSignupType.CantCome)
                {
                    continue;
                }

                if (signup?.Where is AkSignupType.Halan or AkSignupType.Direct)
                {
                    candidates.Add(new SameDayNotificationCandidate(
                        evt.Id,
                        member.Id));
                    continue;
                }

                var isBalletMember = balletMemberIds.Contains(member.Id);

                var relevantByMembership = evt.Type switch
                {
                    AkEventTypes.Rep => !isBalletMember,
                    AkEventTypes.BalettRep => isBalletMember,
                    AkEventTypes.KarRep => true,
                    AkEventTypes.AthenRep => true,
                    AkEventTypes.Samlingsrep => true,
                    AkEventTypes.FikaRep => true,
                    _ => false
                };

                if (relevantByMembership)
                {
                    candidates.Add(new SameDayNotificationCandidate(
                        evt.Id,
                        member.Id));
                }
            }
        }

        return candidates;
    }
}
