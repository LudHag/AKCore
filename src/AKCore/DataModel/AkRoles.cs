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
}

