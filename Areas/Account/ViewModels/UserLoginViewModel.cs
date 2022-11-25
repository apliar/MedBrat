using System.ComponentModel.DataAnnotations;

namespace MedBrat.Areas.Account.ViewModels
{
    public class UserLoginViewModel
    {
        [Required(ErrorMessage = "Не указан номер полиса")]
        [Display(Name = "Номер полиса")]
        public string? Polis { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string? Password { get; set; }
    }
}
