namespace MedBrat.Areas.Appointment.ViewModels
{
    public class ScheduleViewModel
    {
        public string DoctorName { get; set; }
        public Dictionary<DateTime, Dictionary<TimeSpan, bool>> Schedule { get; set; }
            = new Dictionary<DateTime, Dictionary<TimeSpan, bool>>();
    }
}
