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

        public string Name { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public UserRole MemberRole { get; set; }

        public UserStatus Status { get; set; } = UserStatus.Active;

        public  string Password { get; set; }

        public int MembershipTypesId { get; set; }

        // Navigation
        public MembershipTypes MembershipType { get; set; }
        public UserStat? UserStat { get; set; }
        
        public User() { }
        public User(string name, string email, string phonenumber, string password, UserRole userRole, int membershipid)
        {
            Name = name;
            Email = email;
            PhoneNumber = phonenumber;
            Password = password;
            MemberRole = userRole;
            MembershipTypesId = membershipid;
        }

        override public string ToString()
        {
            return $"UserId: {UserId}, Name: {Name}, Email: {Email}, PhoneNumber: {PhoneNumber}, Role: {MemberRole}, Status: {Status}, MembershipId: {MembershipTypesId}";
        }
    }
}
