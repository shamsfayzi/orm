using System.Diagnostics;

namespace Operational_Risk_Management.Models.Extensions
{
    public static class DatetimeExtensions
    {
        public static string ToComplateDatetime(this DateTime value)
        {
            return value.ToString("dd MMM yyyy HH:mm");
        }
        public static DateTime ToDatetime(this DateTime? value)
        {
            return DateTime.Parse(value.ToString());
        }
        public static DateTime LastDayOfTheYear(this DateTime value)
        {
            DateTime n = new DateTime(value.Year + 1, 1, 1);
            return n.AddDays(-1);
        }

        /// <summary>
        /// if value is null returns current datetime
        /// </summary>
        /// <param name="value"></param>
        /// <returns>datetime</returns>
        public static DateTime ToNowIfNull(this DateTime? value)
        {
            return value ?? DateTime.Now;
        }
        /// <summary>
        /// converts total minutes to timespan eg: 12:54
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string MinutesToHour(this double totalMinutes)
        {
            TimeSpan spWorkMin = TimeSpan.FromMinutes(totalMinutes);
            return spWorkMin.ToString(@"hh\:mm");
        }
        public static DateTime ToFirstDateOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }
        public static DateTime ToLastDateOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
        }

    }
}
