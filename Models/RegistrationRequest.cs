using blog_api.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace blog_api.Models
{
    public class RegistrationRequest
    {
        [EmailAddress(ErrorMessage ="Invalid Email Address!")]
        public string? Email { get; set; }

        [Required(ErrorMessage ="Please enter a name")]
        public string? Username { get; set; }

        [Required(ErrorMessage ="Password is required")]
        [StringLength(255, ErrorMessage ="Must be between 5 and 255 characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [NotMapped]
        public string? ConfirmPassword { get; set; }

        [Required]
        public Role Role { get; set; }
    }
}
