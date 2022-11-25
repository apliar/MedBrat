using System.ComponentModel.DataAnnotations;

namespace MedBrat.Areas.Account.ViewModels
{
    public class UserRegistrationViewModel
    {
        [Required(ErrorMessage = "Не указано ФИО")]
        [RegularExpression(@"[A-Za-zА-Яа-я\s]{2,50}", ErrorMessage = "Минимум 2 буквы латинского или русского алфавита")]
        [Display(Name = "ФИО")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Не указан номер полиса")]
        [RegularExpression(@"\d{16}", ErrorMessage = "Номер полиса состоит из 16 цифр")]
        [Display(Name = "Номер полиса")]
        public string? Polis { get; set; }

        [EmailAddress(ErrorMessage = "Некорректный адрес электронной почты")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Адрес электронной почты")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Укажите дату рождения")]
        [DataType(DataType.Date)]
        [Display(Name = "Дата рождения")]
        public DateTime DateOfBirth { get; set; }

        public string? Sex { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [StringLength(18, MinimumLength = 3, ErrorMessage = "Длина пароля должна быть от 3 до 18 символов")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Повторите пароль")]
        public string? PasswordConfirm { get; set; }
    }
}
