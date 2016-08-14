using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AKCore.DataModel;

namespace AKCore.Models
{
    public class AdminEventModel
    {
        public List<Event> Events { get; set; }
        public int Id { get; set; }
        [Display(Name = "Namn")]
        public string Name { get; set; }
        [Display(Name = "Typ")]
        public string Type { get; set; }
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }
        [Display(Name = "Dag")]
        public DateTime Day { get; set; }
        [Display(Name = "Vid hålan")]
        public DateTime Halan { get; set; }
        [Display(Name = "På plats")]
        public DateTime There { get; set; }
        [Display(Name = "Spelning startar")]
        public DateTime Starts { get; set; }
        [Display(Name = "Ståspelning")]
        public bool Stand { get; set; }
    }
}