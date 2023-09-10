using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AKCore.Models
{
    public class AdminEventModel
    {
        public IEnumerable<EventViewModel> Events { get; set; }
        public int Id { get; set; }
        public bool Secret { get; set; }
        [Display(Name = "Namn")]
        public string Name { get; set; }
        [Display(Name = "Plats")]
        public string Place { get; set; }
        [Display(Name = "Typ")]
        public string Type { get; set; }
        [Display(Name = "Fika")]
        public string Fika { get; set; }
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }
        public string DescriptionEng { get; set; }
        [Display(Name = "Intern beskrivning")]
        public string InternalDescription { get; set; }
        public string InternalDescriptionEng { get; set; }
        [Display(Name = "Dag")]
        public DateTime Day { get; set; }
        [Display(Name = "Vid hålan")]
        public TimeSpan Halan { get; set; }
        [Display(Name = "På plats")]
        public TimeSpan There { get; set; }
        [Display(Name = "Spelning")]
        public TimeSpan Starts { get; set; }
        [Display(Name = "Stå- eller gåspelning")]
        public string Stand { get; set; }

        public bool Old { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}