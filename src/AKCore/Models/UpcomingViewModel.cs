using System;
using System.Collections.Generic;
using System.Linq;

namespace AKCore.Models
{
    public class UpcomingViewModel
    {
        public IEnumerable<YearList> Years { get; set; }
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
        public int Year { get; set; }
        public int Month { get; set; }
        public string Day { get; set; }
        public int DayInMonth { get; set; }
        public string HalanTime { get; set; }
        public string ThereTime { get; set; }
        public string StartsTime { get; set; }
        public string Stand { get; set; }
        public string SignupState { get; set; }
        public int Coming { get; set; }
        public int NotComing { get; set; }
    }

    public class YearList
    {
        public int Year { get; set; }
        public IEnumerable<IGrouping<int, EventViewModel>> Months { get; set; }
    }
}