using System;
using System.Collections.Generic;
using System.Linq;
using AKCore.DataModel;

namespace AKCore.Models
{
    public class UpcomingViewModel
    {
        public IEnumerable<EventViewModel> Events { get; set; }
        public bool LoggedIn { get; set; }
        public bool Medlem { get; set; }
        public string IcalLink { get; set; }
    }

    public class EventViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Place { get; set; }
        public string Description { get; set; }
        public string InternalDescription { get; set; }
        public string Fika { get; set; }
        public DateTime Day { get; set; }
        public TimeSpan HalanTime { get; set; }
        public TimeSpan ThereTime { get; set; }
        public TimeSpan StartsTime { get; set; }
        public string Stand { get; set; }
        public string SignupState { get; set; }
        public int Coming { get; set; }
        public int NotComing { get; set; }
    }
}