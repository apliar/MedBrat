using System.ComponentModel.DataAnnotations;

namespace MedBrat.Areas.Account.ViewModels
{
    public class RecordCreationViewModel
    {
        [Required(ErrorMessage = "Укажите имя пациента")]
        [Display(Name = "Имя пациента")]
        public string PatientName { get; set; }

        [Required(ErrorMessage = "Укажите номер полиса пациента")]
        [Display(Name = "Номер полиса")]
        public string PatientPolis { get; set; }
    }
}
