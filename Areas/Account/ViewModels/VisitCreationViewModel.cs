using System.ComponentModel.DataAnnotations;

namespace MedBrat.Areas.Account.ViewModels
{
    public class VisitCreationViewModel
    {
        [Required(ErrorMessage = "Укажите имя пациента")]
        [Display(Name = "Имя пациента")]
        public string PatientName { get; set; }

        [Required(ErrorMessage = "Укажите номер полиса пациента")]
        [Display(Name = "Номер полиса")]
        public string PatientPolis { get; set; }

        [Display(Name = "Тип посещения")]
        public string VisitStatus { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Дата посещения")]
        public DateTime VisitDate { get; set; } = DateTime.Today;

        [Display(Name = "Заключение")]
        public string? VisitConclusion { get; set; }
    }
}
