using System.Collections.Generic;

namespace AKCore.DataModel
{
    public class AkRoles
    {
        public const string SuperNintendo = "SuperNintendo";
        public const string Medlem = "Medlem";
        public const string Editor = "Editor";
        public static readonly List<string> Roles = new List<string>()
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
        public static readonly List<string> Instrument = new List<string>()
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
}

