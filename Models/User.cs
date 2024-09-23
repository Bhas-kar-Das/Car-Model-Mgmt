
namespace DemoAppAdo.Models
{
    public class User
    {
        public int UserId { get; set; } // Primary Key

        public string Username { get; set; } // Unique username

        public string PasswordHash { get; set; } // Hashed password

        public string Email { get; set; } // User's email address

        public int RoleId { get; set; } // Foreign key for the role

        public DateTime CreatedOn { get; set; } // Timestamp when the user was created

        public string CreatedBy { get; set; } // Who created the user

        public DateTime UpdatedOn { get; set; } // Timestamp when the user was last updated

        public string UpdatedBy { get; set; } // Who last updated the user

        public string CId { get; set; } // Unique identifier in the format ddmmhhssff
    }
}
