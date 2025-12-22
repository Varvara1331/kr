using System.ComponentModel.DataAnnotations;

namespace TempLinkApp.ViewModels
{
    public class CreateLinkViewModel
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        [Display(Name = "Email сотрудника")]
        public string EmployeeEmail { get; set; }

        [Required(ErrorMessage = "ФИО обязательно")]
        [Display(Name = "ФИО сотрудника")]
        [StringLength(100, ErrorMessage = "ФИО не должно превышать 100 символов")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [Display(Name = "Пароль сотрудника")]
        [StringLength(10, ErrorMessage = "Пароль не должен превышать 10 символов")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Длительность обязательна")]
        [Range(1, 1440, ErrorMessage = "Длительность должна быть от 1 до 1440 минут")]
        [Display(Name = "Длительность (в минутах)")]
        public int DurationMinutes { get; set; } = 60;
    }
}