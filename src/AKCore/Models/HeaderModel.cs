using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace AKCore.Models
{
    public class HeaderModel
    {
        public IList Menus { get; set; }
        public bool LoggedIn { get; set; }
        public string UserName { get; set; }
    }
}