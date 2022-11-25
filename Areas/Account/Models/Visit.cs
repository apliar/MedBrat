namespace MedBrat.Areas.Account.Models
{
    public class Visit
    {
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateTime Date { get; set; }
        public string? Conclusion { get; set; }
        public string Status { get; set; }
    }
}
