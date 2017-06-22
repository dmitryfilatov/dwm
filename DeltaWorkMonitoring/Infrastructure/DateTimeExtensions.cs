using DeltaWorkMonitoring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeltaWorkMonitoring.Infrastructure
{
    public static class DateTimeExtensions
    {
        public static DateTime GetDayStartDate(this DateTime to)
        {
            return new DateTime(to.Year, to.Month, to.Day);
        }

        public static DateTime GetWeekStartDate(this DateTime to)
        {
            DateTime d = new DateTime(to.Year, to.Month, to.Day);
            while (!d.DayOfWeek.Equals(DayOfWeek.Monday))
            {
                d = d.AddDays(-1);
            }
            return d;
        }

        public static DateTime GetMonthStartDate(this DateTime to)
        {
            return new DateTime(to.Year, to.Month, 1);
        }

        public static DateTime GetQuarterStartDate(this DateTime to)
        {
            int currQuarter = (to.Month - 1) / 3 + 1;
            return new DateTime(to.Year, 3 * currQuarter - 2, 1);
        }

        public static DateTime GetYearStartDate(this DateTime to)
        {
            return new DateTime(to.Year, 1, 1);
        }

        public static DateTime GetRandomDateTime(this DateTime to, TaskPeriod period)
        {
            switch (period)
            {
                case TaskPeriod.Day:
                    return GetRandomDate(to.GetDayStartDate(), to);
                case TaskPeriod.Week:
                    return GetRandomDate(to.GetWeekStartDate(), to);
                case TaskPeriod.Month:
                    return GetRandomDate(to.GetMonthStartDate(), to);
                case TaskPeriod.Quarter:
                    return GetRandomDate(to.GetQuarterStartDate(), to);
                case TaskPeriod.Year:
                    return GetRandomDate(to.GetYearStartDate(), to);
                case TaskPeriod.All:
                    return GetRandomDate(to.AddYears(-100), to);
                default:
                    throw new ArgumentException(nameof(period));
            }
        }

        static readonly Random rnd = new Random();
        public static DateTime GetRandomDate(DateTime from, DateTime to)
        {
            var range = to - from;
            var randTimeSpan = new TimeSpan((long)(rnd.NextDouble() * range.Ticks));
            return from + randTimeSpan;
        }
    }
}
