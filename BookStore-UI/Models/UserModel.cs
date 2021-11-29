using System.ComponentModel.DataAnnotations;

namespace BookStore_UI.Models
{
    public class RegistrationModel
    {
        [Required]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
        
        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(15, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Password Confirm")]
        [DataType(DataType.Password)]
        [StringLength(15, MinimumLength = 6)]
        public string ConfirmPassword { get; set; }
    }


    public class LoginModel
    {
    }
}
