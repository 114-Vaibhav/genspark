namespace librarymanagement.ModelLib
{

    public class Member
    {
        public int MemberId { get; set; }=0;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime MembershipJoinedDate { get; set; } = DateTime.Now;

    }
}
