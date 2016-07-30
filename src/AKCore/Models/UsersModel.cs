using System.Collections;
using System.Collections.Generic;
using AKCore.DataModel;

namespace AKCore.Models
{
    public class UsersModel
    {
        public List<AkUser> Users { get; set; }
        public string SearchPhrase { get; set; }
        public Hashtable Roles { get; set; }
        public Hashtable Posts { get; set; }

        public UsersModel()
        {
            Roles=new Hashtable();
            Posts=new Hashtable();
        }
    }
}