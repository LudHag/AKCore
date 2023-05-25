using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.AspNetCore.Identity;

namespace AKCore.DataModel
{
    public class AkUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Adress { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Nation { get; set; }
        public string Phone { get; set; }
        public string Instrument { get; set; }
        public string SlavPoster { get; set; }
        public string Medal { get; set; }
        public string GivenMedal { get; set; }
        public string OtherInstruments { get; set; }
        public DateTime LastSignedIn {  get; set; }
        public bool HasKey { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; } = new List<IdentityUserRole<string>>();

        public string GetEncodedName()
        {
            if (string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName))
            {
                return UserName;
            }
            else
            {
                return HttpUtility.JavaScriptStringEncode(FirstName + " " + LastName);
            }
        }


        public string GetName()
        {
            if (string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName))
            {
                return UserName;
            }
            else
            {
                return FirstName + " " + LastName;
            }
        }
    }

}

