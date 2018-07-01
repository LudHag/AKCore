using System.Collections;
using System.Collections.Generic;
using AKCore.DataModel;

namespace AKCore.Models
{
    public class UsersModel
    {
        public IList<AkUser> Users { get; set; }
        public string SearchPhrase { get; set; }
        public bool Inactive { get; set; }
        public IDictionary<string, IEnumerable<string>> Roles { get; set; }
        public IDictionary<string, IEnumerable<string>> Posts { get; set; }

        public string GetRoleInfo(string role)
        {
            switch (role)
            {
                case AkRoles.SuperNintendo:
                    return "Har rättigheter att redigera all information på webben";
                case AkRoles.Editor:
                    return "Kan redigera sidor, musikalbum, ladda upp filer samt titta på intresseanmälningar";
                case AkRoles.Medlem:
                    return "Kan anmäla sig till spelningar, syns i adressregistret samt kan redigera sin profil";
                case AkRoles.Balett:
                    return "Kan se balettsidor";
            }

            return "";
        }

        public UsersModel()
        {
            Roles = new Dictionary<string, IEnumerable<string>>();
            Posts = new Dictionary<string, IEnumerable<string>>();
        }
    }
}