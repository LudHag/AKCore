using AKCore.DataModel;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace AKCore.Services;


public enum TranslationDomains
{
    Common,
    Header,
    Widgets,
    ErrorPages,
    Posts,
    Instruments
}
public record Translation(string Swedish, string English);
public record DomainTranslations(Dictionary<string, string> translations);

public class TranslationsService
{
    private readonly bool isEnglish;
    public TranslationsService(IHttpContextAccessor httpContextAccessor)
    {
        var languageCookie = httpContextAccessor.HttpContext.Request.Cookies["language"];
        isEnglish = languageCookie == "EN";
    }

    public string Get(TranslationDomains domain, string key)
    {
        return isEnglish ? translations[domain][key].English : translations[domain][key].Swedish;
    }

    public bool IsEnglish()
    {
        return isEnglish;
    }

    private static readonly Dictionary<string, Translation> commonTranslations = new() {
        { "FirstName", new Translation("Förnamn", "First name") },
        { "LastName", new Translation("Efternamn", "Last name") },
        { "Email", new Translation("Epost", "Email") },
        { "Phone", new Translation("Telefonnummer", "Telephone number") },
        { "SelectInstrument", new Translation("Välj instrument", "Select instrument") },
        { "Gigs", new Translation("Spelningar", "Upcoming gigs") },
        { "Upcoming", new Translation("På gång", "Upcoming") },
    };

    private static readonly Dictionary<string, Translation> widgetsTranslations = new() {
        { "HireUs", new Translation("Vill du anlita oss?", "Do you want to hire us?") },
        { "JoinUs", new Translation("Vill du spela eller dansa med oss?", "Do you want to play or dance with us?") },
        { "AnswerWithNumber", new Translation("Svara med siffra", "Answer with a number") },
        { "BotQuestion", new Translation("Vad är 1 + 2?", "What is 1 + 2?") },
        { "Join", new Translation("Gå med", "Join") },
        { "Hire", new Translation("Anlita oss", "Hire us") },
    };
    private static readonly Dictionary<string, Translation> headerTranslations = new() {
        { "LogIn", new Translation("Logga in", "Log in") },
        { "LogOut", new Translation("Logga ut", "Log out") },
        { "UnreadInterested", new Translation("olästa intresseanmälningar", "unread interested") },
        { "LoggedInAs", new Translation("Inloggad som", "Logged in as") }
    };

    private static readonly Dictionary<string, Translation> errorTranslations = new() {
        { "WrongPage", new Translation("Hallå, det här är fel sida dumbom", "Hello, this is the wrong page you nitwit") },
        { "NothingToSee", new Translation("Inget att se här...", "Nothing to see here...") },
    };


    private static readonly Dictionary<string, Translation> instrumentTranslations = new() {
        { AkInstruments.Altsax, new Translation("Altsax", "Alto sax") },
        { AkInstruments.Balett, new Translation("Balett", "Ballet") },
        { AkInstruments.Banjo, new Translation("Banjo", "Banjo") },
        { AkInstruments.Barytonsax, new Translation("Barytonsax", "Baritone sax") },
        { AkInstruments.Dragspel, new Translation("Dragspel", "Accordion")},
        { AkInstruments.Euphonium, new Translation("Euphonium", "Euphonium") },
        { AkInstruments.Flute, new Translation("Flöjt", "Flute") },
        { AkInstruments.Horn, new Translation("Horn", "Horn") },
        { AkInstruments.Klarinett, new Translation("Klarinett", "Clarinet") },
        { AkInstruments.Oboe, new Translation("Oboe", "Oboe") },
        { AkInstruments.Slagverk, new Translation("Slagverk", "Drums") },
        { AkInstruments.Tenorsax, new Translation("Tenorsax", "Tenor sax") },
        { AkInstruments.Trombon, new Translation("Trombon", "Trombone") },
        { AkInstruments.Trumpet, new Translation("Trumpet", "Trumpet") },
        { AkInstruments.Tuba, new Translation("Tuba", "Tuba") },
    };


    private static readonly Dictionary<string, Translation> postsTranslations = new() {
        { "ÖK", new Translation("Ordförande", "Chairman") },
        { "KK", new Translation("Kassör", "Cashier") },
        { "SK", new Translation("Sekreterare", "Secretary") },
        { "Ballet", new Translation("Ansvariga för baletten", "Responsible for the ballet") },
        { "Music", new Translation("Ansvarig för det konstnärliga - dirigent", "Responsible for the artistic - conductor") },
        { "Pub", new Translation("Ansvarig för puben", "Responsible for the pub") },
        { "Sex", new Translation("Ansvarig för allt vad fest heter", "Responsible for parties") },
        { "Arsenal", new Translation("Ansvarig för orkesterns instrument och reparation av dessa", "Responsible for orchestras instruments and the reparations for them") },
        { "Bus", new Translation("Ansvariga för grisen", "Responsible for the pig (bus)") },
        { "Camera", new Translation("Ansvara för att dokumentera AKs frammarsch i form av foto och film", "Responsible for documenting AK in photo and video") },
        { "Hug", new Translation("Har ansvar för att agera som en trygg kontakt för föreningens medlemmar under verksamheten", "Responsible for acting like a safe contact for the members of the organisation") },
        { "Cozy", new Translation("Ansvarig för att det ska vara mysigt i hålan(dvs toapapper och sånt)", "Responsible for making Hålan cozy(toilet paper etc)") },
        { "Nintendo", new Translation("Ansvarig för Hålans dator, AKs hemsida samt för info- och snacklistan.", "Responsible for the computer in Hålan, AKs website, info and snack email lists") },
        { "Nostalgia", new Translation("Ansvarig för orkesterns kollektiva minne", "Responsible for the orchestras collective memory") },
        { "Note", new Translation("Ansvarig för notarkivet", "Responsible for note archive") },
        { "PR", new Translation("Ansvarar för att vi hörs och syns via t.ex. affischer", "Responsible for us being heard and seen via for example posters") },
        { "Gadget", new Translation("Ansvarig för medaljer, tröjor, märken och dylikt", "Responsible for medals, t-shirts, brands etc.") },
        { "Scrub", new Translation("Ansvarig för Hålan, t.ex. glödlampor osv.", "Responsible for Hålan, for exampel light bulbs etc.") },
        { "Sponsor", new Translation("Ansvarar för att hitta sponsorer", "Responsible for finding sponsors") },
        { "Board", new Translation("Styrelse",  "Board") },
        { "Functionaries", new Translation("Funktionärer",  "Functionaries") },
        { "OtherPosts", new Translation("Slavkamererer", "Other posts") }

    };



    private static readonly Dictionary<TranslationDomains, Dictionary<string, Translation>> translations = new() {
        { TranslationDomains.Common, commonTranslations},
        { TranslationDomains.Widgets, widgetsTranslations },
        { TranslationDomains.Header, headerTranslations },
        { TranslationDomains.ErrorPages, errorTranslations },
        { TranslationDomains.Posts, postsTranslations },
        { TranslationDomains.Instruments, instrumentTranslations },
    };

}

