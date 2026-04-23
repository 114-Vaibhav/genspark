using System.ComponentModel.DataAnnotations;
namespace backend.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DOB { get; set; }

        public string Role { get; set; } = string.Empty; // Admin / Operator / User
        public string PasswordHash { get; set; } = string.Empty;
    }
}
