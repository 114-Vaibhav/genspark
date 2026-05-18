namespace librarymanagementsystem.ModelLib
{
    public class MembershipTypes
    {
        public int MembershipTypesId { get; set; }

        public required string MembershipName { get; set; }

        public int MaxBooksAllowed { get; set; }

        public int MaxBorrowDays { get; set; }

        // Navigation
        public ICollection<User> Users { get; set; } = new List<User>();

        override public string ToString()
        {
            return $"MembershipTypesId: {MembershipTypesId}, MembershipName: {MembershipName}, MaxBooksAllowed: {MaxBooksAllowed}, MaxBorrowDays: {MaxBorrowDays}";
        }
    }
}