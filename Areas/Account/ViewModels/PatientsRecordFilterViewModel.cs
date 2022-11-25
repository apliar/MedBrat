namespace MedBrat.Areas.Account.ViewModels
{
    public class PatientsRecordFilterViewModel
    {
        public List<PatientsRecordViewModel> Patients { get; }
        public string SelectedName { get; }

        public PatientsRecordFilterViewModel(List<PatientsRecordViewModel> patients, string name)
        {
            Patients = patients;
            SelectedName = name;
        }
    }
}
