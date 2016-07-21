using System.Collections.Generic;
using System.ComponentModel;

namespace AKCore.Models
{
    public class ProfileModel
    {
        [DisplayName("Användarnamn")]
        public string UserName { get; set; }
        [DisplayName("Epost")]
        public string Email { get; set; }
        public IList<string> Roles { get; set; }

        public ProfileModel()
        {
            Roles=new List<string>();
        }
    }
}