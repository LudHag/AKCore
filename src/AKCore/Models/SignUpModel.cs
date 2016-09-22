using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AKCore.DataModel;

namespace AKCore.Models
{
    public class SignUpModel
    {
        [Display(Name = "Kommer")]
        [Required]
        public string Where { get; set; }
        [Display(Name = "Har bil")]
        public bool Car { get; set; }
        [Display(Name = "Tar med instrument själv")]
        public bool Instrument { get; set; }
        [Display(Name = "Kommentar")]
        public string Comment { get; set; }

        public Event Event { get; set; }
    }
}