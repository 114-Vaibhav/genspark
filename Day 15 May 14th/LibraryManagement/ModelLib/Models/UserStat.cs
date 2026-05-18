namespace librarymanagementsystem.ModelLib
{
    public class UserStat
    {
        public int UserStatId { get; set; }
        public int UserId { get; set; }

        public int BooksBorrowed { get; set; } = 0;

        public int BooksReturned { get; set; } = 0;

        public int BooksOverdue { get; set; } = 0;

        public decimal TotalUnpaidFines { get; set; } = 0;

        public decimal TotalFinesPaid { get; set; } = 0;

        // Navigation
        public User User { get; set; }

        public override string ToString()
        {
            return $"UserId: {UserId}, BooksBorrowed: {BooksBorrowed}, BooksReturned: {BooksReturned}, BooksOverdue: {BooksOverdue}, TotalUnpaidFines: {TotalUnpaidFines}, TotalFinesPaid: {TotalFinesPaid}";
        }
    }
}