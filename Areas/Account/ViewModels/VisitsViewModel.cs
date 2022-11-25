using MedBrat.Areas.Account.Models;

namespace MedBrat.Areas.Account.ViewModels
{
    public class VisitsViewModel
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public List<Visit> Visits { get; set; }
    }
}
