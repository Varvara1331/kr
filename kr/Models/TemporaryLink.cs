using System;
using System.ComponentModel.DataAnnotations;

namespace demo.Models
{
    public class TemporaryLink
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [EmailAddress]
        public string EmployeeEmail { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Password { get; set; }

        public string Phone { get; set; } = string.Empty;

        public string InternalNumber { get; set; } = string.Empty;

        public string Position { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; } = false;
        public DateTime? UsedAt { get; set; }
    }
}