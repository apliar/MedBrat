using System.ComponentModel.DataAnnotations;

namespace MedBrat.Areas.Account.ViewModels
{
    public class DoctorAddViewModel : UserRegistrationViewModel
    {
        [Display(Name = "Логин для входа")]
        public string Polis { get; set; }
        public string Specialization { get; set; }
    }
}
