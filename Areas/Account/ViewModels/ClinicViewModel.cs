namespace MedBrat.Areas.Account.ViewModels
{
    public class ClinicViewModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public List<DoctorsRecordViewModel> Doctors { get; set; } = new List<DoctorsRecordViewModel>();
    }
}
