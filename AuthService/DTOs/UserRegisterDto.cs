using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs
{
    public class UserRegisterDto
    {
        [Required, StringLength(100, MinimumLength = 3)]
        public string Username { get; set; } = null!;

        [Required, StringLength(100)]
        public string FirstName { get; set; } = null!;
        [Required, StringLength(100)]
        public string LastName { get; set; } = null!;


        [Required, StringLength(128, MinimumLength = 8)]
        public string Password { get; set; } = null!;
        [Required, EmailAddress, StringLength(254)]
        public string Email { get; set; } = null!;
        // Kept for existing clients; public registration ignores this value.
        public string? Role { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? BirthDate { get; set; }
    }
}