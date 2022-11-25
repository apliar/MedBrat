namespace MedBrat.Areas.Appointment.Models
{
    public class ScheduleGenerator
    {
        int day;
        int month;
        int year;
        Dictionary<DateTime, List<TimeSpan>> schedule = new Dictionary<DateTime, List<TimeSpan>>();

        public Dictionary<DateTime, List<TimeSpan>> Generate(DateTime startDate, int weeksCount, 
            Dictionary<int, List<TimeSpan>> pattern)
        {
            day = startDate.Day;
            month = startDate.Month;
            year = startDate.Year;
            schedule.Clear();

            addWeekToSchedule((int)startDate.DayOfWeek, pattern);
            for (var i = 0; i < weeksCount - 1; i++)
            {
                addWeekToSchedule(1, pattern);
            }

            return schedule;
        }

        public List<DateOnly> GenerateDates(DateTime startDate, int weeksCount)
        {
            day = startDate.Day;
            month = startDate.Month;
            year = startDate.Year;

            var dates = new List<DateOnly>();

            dates.Add(new DateOnly(year, month, day));
            addDay();

            for(var i = 1; i < weeksCount * 7; i++)
            {
                dates.Add(new DateOnly(year, month, day));
                addDay();
            }

            return dates;
        }

        private void addWeekToSchedule(int startDay, Dictionary<int, List<TimeSpan>> pattern)
        {
            for(var i = startDay; i <= 5; i++)
            {
                var date = new DateTime(year, month, day);
                if (pattern.ContainsKey(i))
                    schedule[date] = pattern[i].Take(pattern[i].Count - 1).ToList();

                addDay();
            }
            addDay();
            addDay();
        }

        private void addDay()
        {
            if(++day > DateTime.DaysInMonth(year, month))
            {
                day = 1;
                addMonth();
            }
        }
        private void addMonth()
        {
            if(month++ > 12)
            {
                month = 1;
                year++;
            }
        }
    }
}
