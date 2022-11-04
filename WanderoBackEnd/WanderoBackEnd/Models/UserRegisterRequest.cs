using System.ComponentModel.DataAnnotations;

namespace WanderoBackEnd.Models
{
    public class UserRegisterRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(8, ErrorMessage = "Please enter a password that contains at least 8 characters!")]
        public string Password { get; set; } = string.Empty;
        [Required, Compare("Password")]
        public string ConfirmedPassword { get; set; } = string.Empty;
    }
}
