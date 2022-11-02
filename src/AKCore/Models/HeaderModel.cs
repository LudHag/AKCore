using System.Collections.Generic;

namespace AKCore.Models
{
    public class HeaderModel
    {
        public IList<ModelMenu> Menus { get; set; }
        public bool LoggedIn { get; set; }
        public string UserName { get; set; }
        public string CurrentUrl { get; set; }
        public bool Member { get; set; }
        public int Recruits { get; set; }
        public int Mails { get; set; }
    }
}