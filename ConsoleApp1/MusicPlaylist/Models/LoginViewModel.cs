using System.ComponentModel.DataAnnotations;

namespace MusicPlaylist.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(
            20,
            MinimumLength = 3,
            ErrorMessage = "Username must be between 3 and 20 characters."
        )]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(
            6,
            ErrorMessage = "Password must be at least 6 characters."
        )]
        [MaxLength(
            16,
            ErrorMessage = "Password cannot exceed 16 characters."
        )]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}