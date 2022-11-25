namespace MedBrat.Areas.Appointment.ViewModels
{
    public class ScheduleEditViewModel
    {
        public int Id { get; set; }
        public List<ScheduleDayEditViewModel> Days { get; set; } = 
            new List<ScheduleDayEditViewModel>();
    }
}
