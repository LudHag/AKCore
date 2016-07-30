using System.Collections.Generic;
using AKCore.DataModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AKCore.DataModel
{
    public class AkUser : IdentityUser
    {
        public string GoogleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Adress { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Nation { get; set; }
        public string Instrument { get; set; }
        public string SlavPoster { get; set; }
    }
    
}

