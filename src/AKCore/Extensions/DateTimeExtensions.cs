using System;

namespace AKCore.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ConvertToSwedishTime(this DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));
        }
    }
}
