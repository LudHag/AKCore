using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace AKCore.Services;


public enum TranslationDomains
{
    Header,
    Hire
}
public record Translation(string Swedish, string English);
public record DomainTranslations(Dictionary<string, string> translations);

public class TranslationsService
{
    private bool isEnglish;
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

    private static readonly Dictionary<string, Translation> hireTranslations = new() {
        { "HireUs", new Translation("Vill du anlita oss?", "Do you want to hire us?") }
    };
    private static readonly Dictionary<string, Translation> headerTranslations = new() {
        { "LogIn", new Translation("Logga in", "Log in") },
        { "LogOut", new Translation("Logga ut", "Log out") },
        { "UnreadInterested", new Translation("olästa intresseanmälningar", "unread interested") }
    };

    private static readonly Dictionary<TranslationDomains, Dictionary<string, Translation>> translations = new() {
       { TranslationDomains.Hire, hireTranslations },
       { TranslationDomains.Header, headerTranslations },
    };

}

