using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AKCore.DataModel;

namespace AKCore.Models
{
    public class AdminEventModel
    {
        public IList<Event> Events { get; set; }
        public int Id { get; set; }
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
        [Display(Name = "Intern beskrivning")]
        public string InternalDescription { get; set; }
        [Display(Name = "Dag")]
        public DateTime Day { get; set; }
        [Display(Name = "Vid hålan")]
        public DateTime Halan { get; set; }
        [Display(Name = "På plats")]
        public DateTime There { get; set; }
        [Display(Name = "Spelning")]
        public DateTime Starts { get; set; }
        [Display(Name = "Stå- eller gåspelning")]
        public string Stand { get; set; }
    }
}