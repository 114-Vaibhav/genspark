namespace librarymanagementsystem.ModelLib
{
    public enum UserStatus
    {
        Active,
        Inactive,
        Banned
    }

    public enum UserRole
    {
        Admin,
        Basic,
        Premium,
        Student
    }

    public class User
    {
        public int UserId { get; set; }

        public required string Name { get; set; }

        public required string Email { get; set; }

        public required string PhoneNumber { get; set; }

        public UserRole MemberRole { get; set; }

        public UserStatus Status { get; set; } = UserStatus.Active;

        public required string Password { get; set; }

        public int MembershipId { get; set; }

        // Navigation
        public MembershipTypes MembershipType { get; set; }
        public UserStat? UserStat { get; set; }


        
    }
}
