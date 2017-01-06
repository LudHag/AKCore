using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AKCore.DataModel;

namespace AKCore.Models
{
    public class JoinUsModel
    {
        [Display(Name = "Förnamn")]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Efternamn")]
        [Required]
        public string LastName { get; set; }

        [Display(Name = "Epost")]
        public string Email { get; set; }

        [Display(Name = "Telefonnummer")]
        public string Tel { get; set; }

        [Display(Name = "Instrument")]
        public string Instrument { get; set; }

        [Display(Name = "Övrigt")]
        public string Other { get; set; }
    }
}