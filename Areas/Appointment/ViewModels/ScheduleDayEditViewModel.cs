namespace MedBrat.Areas.Appointment.ViewModels
{
    public class ScheduleDayEditViewModel
    {
        public int Day { get; set; }
        public bool IsWorkingDay { get; set; }
        public string WorkingHoursStart { get; set; }
        public string WorkingHoursEnd { get; set; }
        public int? TicketsNumber { get; set; }
        public int? TicketsInterval { get; set; }
    }
}
