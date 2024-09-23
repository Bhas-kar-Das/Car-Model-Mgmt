namespace DemoAppAdo.DTOs
{
    using System.ComponentModel.DataAnnotations;

    namespace DemoAppAdo.DTOs
    {
        public class CreateUserDto
        {
            [Required(ErrorMessage = "Username is required.")]
            [StringLength(50, ErrorMessage = "Username can't be longer than 50 characters.")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Password is required.")]
            [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
            public string PasswordHash { get; set; }

            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Role ID is required.")]
            [Range(1, int.MaxValue, ErrorMessage = "Role ID must be a positive integer.")]
            public int RoleId { get; set; }
        }
    }

}
