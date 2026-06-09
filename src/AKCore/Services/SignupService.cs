using AKCore.DataModel;
using AKCore.Extensions;
using AKCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AKCore.Services;

public class SignupService
{
    private readonly AKContext _db;
    private readonly UserManager<AkUser> _userManager;

    public SignupService(AKContext db, UserManager<AkUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public Event GetEventWithSignups(int eventId) =>
        _db.Events.Include(x => x.SignUps).FirstOrDefault(x => x.Id == eventId);

    public static List<string> GetInstrumentsForUser(AkUser user)
    {
        var instruments = new List<string>();
        if (!string.IsNullOrWhiteSpace(user.Instrument))
        {
            instruments.Add(user.Instrument);
        }

        if (!string.IsNullOrWhiteSpace(user.OtherInstruments))
        {
            foreach (var instr in user.OtherInstruments.Split(','))
            {
                var trimmed = instr.Trim();
                if (!string.IsNullOrWhiteSpace(trimmed) && !instruments.Contains(trimmed))
                {
                    instruments.Add(trimmed);
                }
            }
        }

        return instruments;
    }

    public async Task<SignUpModel> BuildSignUpModelAsync(Event spelning, AkUser user, bool isNintendo, bool isEnglish)
    {
        var model = new SignUpModel();
        var signup = spelning.SignUps.Select(x => x.CopySignupWithoutEvent())
            .FirstOrDefault(x => x.PersonId == user.Id);

        if (signup != null)
        {
            model.Where = signup.Where;
            model.Car = signup.Car;
            model.Instrument = signup.Instrument;
            model.Comment = signup.Comment;
            model.SelectedInstrument = signup.InstrumentName;
        }

        var instruments = GetInstrumentsForUser(user);
        model.AvailableInstruments = instruments;
        if (model.SelectedInstrument == null && instruments.Count > 0)
        {
            model.SelectedInstrument = instruments[0];
        }

        model.IsNintendo = isNintendo;
        model.Event = EventService.MapEventModelForUpcoming(spelning, true, user.Id, isEnglish);
        model.Signups = spelning.SignUps.Select(x => x.CopySignupWithoutEvent())
            .OrderBy(x => x.InstrumentName).ThenBy(x => x.PersonName);
        model.IsPassed = spelning.Day.Date < DateTime.UtcNow.Date.AddDays(-1);

        if (isNintendo)
        {
            model.Members = (await _userManager.GetUsersInRoleAsync(AkRoles.Medlem))
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .Select(x => new MemberViewModel
                {
                    Id = x.Id,
                    FullName = x.GetName()
                });
        }

        return model;
    }

    public async Task SaveSignupAsync(SignUpModel model, int eventId, AkUser user)
    {
        var spelning = await _db.Events.Include(x => x.SignUps).FirstOrDefaultAsync(x => x.Id == eventId);
        if (spelning == null)
        {
            throw new AkValidationError("InvalidId");
        }

        if (string.IsNullOrWhiteSpace(model.Where))
        {
            throw new AkValidationError("MustChooseWhere");
        }

        var signup = spelning.SignUps.FirstOrDefault(x => x.PersonId == user.Id) ?? new SignUp();
        if (signup.Where == AkSignupType.CantCome || model.Where == AkSignupType.CantCome)
        {
            signup.SignupTime = DateTime.Now;
        }
        else
        {
            signup.SignupTime = signup.Where == null ? DateTime.Now : signup.SignupTime;
        }

        signup.Where = model.Where;
        signup.Car = model.Car;
        signup.Instrument = model.Instrument;
        signup.Comment = model.Comment;
        signup.Person = user.UserName;
        signup.PersonId = user.Id;
        signup.PersonName = user.GetName();
        signup.OtherInstruments = null;

        var allInstruments = GetInstrumentsForUser(user);
        if (!string.IsNullOrWhiteSpace(model.SelectedInstrument) &&
            allInstruments.Contains(model.SelectedInstrument.Trim()))
        {
            signup.InstrumentName = model.SelectedInstrument.Trim();
        }
        else
        {
            signup.InstrumentName = user.Instrument;
        }

        spelning.SignUps.Add(signup);
        await _db.SaveChangesAsync();
    }

    public async Task EditSignupAsync(int eventId, string memberId, string type, bool instrument, bool car)
    {
        if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(memberId))
        {
            throw new AkValidationError("InvalidData");
        }

        var e = await _db.Events.Include(x => x.SignUps).FirstOrDefaultAsync(x => x.Id == eventId);
        if (e == null)
        {
            throw new AkValidationError("InvalidData");
        }

        var member = await _db.Users.FirstOrDefaultAsync(x => x.Id == memberId);
        if (member == null)
        {
            throw new AkValidationError("InvalidData");
        }

        var signUp = e.SignUps.FirstOrDefault(x => x.PersonId == member.Id);
        if (signUp != null)
        {
            signUp.Where = type;
            signUp.InstrumentName = member.Instrument;
            signUp.Instrument = instrument;
            signUp.Car = car;
        }
        else
        {
            e.SignUps.Add(new SignUp
            {
                Person = member.UserName,
                PersonName = member.GetName(),
                PersonId = member.Id,
                SignupTime = DateTime.Now.ConvertToSwedishTime(),
                Where = type,
                InstrumentName = member.Instrument
            });
        }

        await _db.SaveChangesAsync();
    }
}
