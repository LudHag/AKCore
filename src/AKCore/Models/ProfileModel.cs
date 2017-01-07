using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AKCore.Models
{
    public class ProfileModel
    {
        [Display(Name = "Användarnamn")]
        public string UserName { get; set; }
        [EmailAddress]
        [Display(Name = "Epost")]
        public string Email { get; set; }
        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }
        [Display(Name = "Efternamn")]
        public string LastName { get; set; }
        [Display(Name = "Adress")]
        public string Adress { get; set; }
        [Display(Name = "Postnummer")]
        public string ZipCode { get; set; }
        [Display(Name = "Stad")]
        public string City { get; set; }
        [Display(Name = "Telefonnummer")]
        public string Phone { get; set; }
        [Display(Name = "Nation")]
        public string Nation { get; set; }
        [Display(Name = "Instrument")]
        public string Instrument { get; set; }

        [Display(Name = "Andra instrument")]
        public IList<string> OtherInstrument { get; set; }


        public string Medal { get; set; }

        public bool Facebook { get; set; }
        public bool Google { get; set; }

        public IList<string> Roles { get; set; }
        public IList<string> Poster { get; set; }
        public ProfileModel()
        {
            Roles=new List<string>();
        }
    }
}