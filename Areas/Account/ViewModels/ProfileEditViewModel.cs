using System.ComponentModel.DataAnnotations;

namespace MedBrat.Areas.Account.ViewModels
{
    public class ProfileEditViewModel
    {
        [Required(ErrorMessage = "Не указано ФИО")]
        [RegularExpression(@"[A-Za-zА-Яа-я\s]{2,50}", ErrorMessage = "Минимум 2 буквы латинского или русского алфавита")]
        [Display(Name = "ФИО")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не указан номер полиса")]
        [Display(Name = "Номер полиса")]
        public string Polis { get; set; }

        [EmailAddress(ErrorMessage = "Некорректный адрес электронной почты")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Адрес электронной почты")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Укажите дату рождения")]
        [RegularExpression(@"\d{2}.\d{2}.\d{4}", ErrorMessage = "Указан неверный формат даты")]
        [Display(Name = "Дата рождения")]
        public string DateOfBirth { get; set; }

        public string? Sex { get; set; }

        [StringLength(18, MinimumLength = 3, ErrorMessage = "Длина пароля должна быть от 3 до 18 символов")]
        [DataType(DataType.Password)]
        [Display(Name = "Старый пароль")]
        public string? OldPassword { get; set; }

        [StringLength(18, MinimumLength = 3, ErrorMessage = "Длина пароля должна быть от 3 до 18 символов")]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string? NewPassword { get; set; }
    }
}
