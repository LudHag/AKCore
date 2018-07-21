using System.Collections;
using System.Collections.Generic;
using AKCore.DataModel;

namespace AKCore.Models
{
    public class UsersViewModel
    {
        public IList<AkUserViewModel> Users { get; set; }
    }

    public class AkUserViewModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Instrument { get; set; }
        public string SlavPoster { get; set; }
        public string Medal { get; set; }
        public string GivenMedal { get; set; }
        public string OtherInstruments { get; set; }
        public bool HasKey { get; set; }
        public bool Active { get; set; }
        public IEnumerable<string> Roles;
        public IEnumerable<string> Posts;
    }
}