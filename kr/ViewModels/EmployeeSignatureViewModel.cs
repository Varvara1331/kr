using System.ComponentModel.DataAnnotations;

namespace demo.ViewModels
{
    public class EmployeeSignatureViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Должность обязательна для заполнения")]
        [Display(Name = "Должность")]
        public string Position { get; set; }

        [Display(Name = "Внутренний номер")]
        public string InternalNumber { get; set; }

        [Display(Name = "Мобильный телефон")]
        public string Phone { get; set; }

        [Required]
        public string Token { get; set; }

        public bool IsSuccess { get; set; } = false;
    }
}