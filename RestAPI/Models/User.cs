using System.ComponentModel.DataAnnotations;

namespace RestAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "Password cannot be longer than 100 characters.")]
        public string PasswordHash { get; set; } = string.Empty;
    }
}
