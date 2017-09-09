using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AKCore.DataModel;

namespace AKCore.Models
{
    public class SignUpModel
    {
        [Display(Name = "Kommer till:")]
        [Required]
        public string Where { get; set; }
        [Display(Name = "Har bil")]
        public bool Car { get; set; }
        [Display(Name = "Tar med instrument själv")]
        public bool Instrument { get; set; }
        [Display(Name = "Kommentar")]
        public string Comment { get; set; }

        public bool IsNintendo { get; set; }

        public IList<AkUser> Members { get; set; }

        public Event Event { get; set; }

        public IList<SignUp> Comming()
        {
            return Event.SignUps.Where(x => x.Where != "Kan inte komma").OrderBy(x=>x.InstrumentName).ToList();
        }
        public IList<SignUp> NotComming()
        {
            return Event.SignUps.Where(x => x.Where == "Kan inte komma").OrderBy(x => x.InstrumentName).ToList();
        }
    }
}