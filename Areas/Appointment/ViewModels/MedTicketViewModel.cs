namespace MedBrat.Areas.Appointment.ViewModels
{
    public class MedTicketViewModel
    {
        public int PatientId { get; set; } 
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool IsExpired { get; set; }
    }
}
