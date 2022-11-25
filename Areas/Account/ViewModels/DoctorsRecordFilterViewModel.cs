namespace MedBrat.Areas.Account.ViewModels
{
    public class DoctorsRecordFilterViewModel
    {
        public string ClinicName { get; }
        public List<DoctorsRecordViewModel> Doctors { get; }
        public string SelectedName { get; }

        public DoctorsRecordFilterViewModel(List<DoctorsRecordViewModel> doctors, string name, string clinic)
        {
            ClinicName = clinic;
            Doctors = doctors;
            SelectedName = name;
        }
    }
}
