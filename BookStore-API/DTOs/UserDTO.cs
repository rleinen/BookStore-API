using System.ComponentModel.DataAnnotations;

namespace BookStore_API.DTOs
{
    public class UserDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [StringLength(15,MinimumLength = 6)]
        public string Password { get; set; }

    }
}
