using System.Collections;
using System.Collections.Generic;

namespace AKCore.DataModel
{
    public class AkRoles
    {
        public const string SuperNintendo = "SuperNintendo";
        public const string Medlem = "Medlem";
        public const string Editor = "Editor";
        public static readonly IList<string> Roles = new List<string>()
        {
            SuperNintendo,
            Medlem,
            Editor
        };
    }
    public class AkInstruments
    {
        public const string Altsax = "Altsax";
        public const string Banjo = "Banjo";
        public const string Balett = "Balett";
        public const string Barytonsax = "Barytonsax";
        public const string Klarinett = "Klarinett";
        public const string Euphonium = "Euphonium";
        public const string Flute = "Flöjt";
        public const string Horn = "Horn";
        public const string Slagverk = "Slagverk";
        public const string Tenorsax = "Tenorsax";
        public const string Tuba = "Tuba";
        public const string Trombon = "Trombon";
        public const string Trumpet = "Trumpet";
        public static readonly IList<string> Instrument = new List<string>()
        {
            Altsax,
            Banjo,
            Balett,
            Barytonsax,
            Klarinett,
            Euphonium,
            Flute,
            Horn,
            Slagverk,
            Tenorsax,
            Tuba,
            Trombon,
            Trumpet
        };
    }
    public class AkPoster
    {
        public const string OK = "Överkamerer";
        public const string SK = "Skrivkamerer";
        public const string KK = "Kamrerskamerer";
        public const string Balettkamerer = "Balettkamerer";
        public const string Busskamerer = "Busskamerer";
        public const string Klimatkamerer = "Klimatkamerer";
        public const string Kamerakamerer = "Kamerakamerer";
        public const string Myskamerer = "Myskamerer";
        public const string Musikkamerer = "Musikkamerer";
        public const string Nintendokamerer = "Nintendokamerer";
        public const string Nostalgikamerer = "Nostalgikamerer";
        public const string Notkamerer = "Notkamerer";
        public const string Prkamerer = "Pr-kamerer";
        public const string Prylkamerer = "Prylkamerer";
        public const string Pubkamerer = "Pubkamerer";
        public const string Sexkamerer = "Sexkamerer";
        public const string Skrubbkamerer = "Skrubbkamerer";
        public const string Sponsringskamerer = "Sponsringskamerer";
        public const string Bardkamerer = "Bardkamerer";
        public const string Moveskamerer = "Moveskamerer";
        public static readonly IList<string> Poster = new List<string>()
        {
            OK,
            SK,
            KK,
            Balettkamerer,
            Busskamerer,
            Klimatkamerer,
            Kamerakamerer,
            Myskamerer,
            Musikkamerer,
            Nintendokamerer,
            Nostalgikamerer,
            Notkamerer,
            Prkamerer,
            Prylkamerer,
            Pubkamerer,
            Sexkamerer,
            Skrubbkamerer,
            Sponsringskamerer,
            Bardkamerer,
            Moveskamerer
        };
    }

    public class AkEventTypes
    {
        public const string Spelning = "Spelning";
        public const string Rep = "Rep";
        public const string Fest = "Fest";

        public static readonly IList<string> Types = new List<string>()
        {
            Spelning,
            Rep,
            Fest
        };
    }
}

