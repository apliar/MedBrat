namespace MedBrat.Areas.Appointment.ViewModels
{
    public class TicketViewModel
    {
        public string DoctorName { get; set; }
        public DateTime Date { get; set; }
        public bool IsOccupied { get; set; }
    }
}
