using AKCore.DataModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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

        public IEnumerable<MemberViewModel> Members { get; set; }

        public EventViewModel Event { get; set; }
        public IEnumerable<SignUp> Signups { get; set; }

        public IList<SignUp> Comming()
        {
            return Signups.Where(x => x.Where != "Kan inte komma").OrderBy(x => x.InstrumentName).ToList();
        }
        public IList<SignUp> NotComming()
        {
            return Signups.Where(x => x.Where == "Kan inte komma").OrderBy(x => x.InstrumentName).ToList();
        }
    }

    public class MemberViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
    }
}