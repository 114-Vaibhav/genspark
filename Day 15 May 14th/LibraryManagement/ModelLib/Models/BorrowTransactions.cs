namespace librarymanagementsystem.ModelLib
{
    public class BorrowTransactions
    {
        public int BorrowTransactionsId { get; set; }
        public int BookId { get; set; }
        public int BookCopyId { get; set; }
        public int UserId { get; set; }
        public DateTime IssueDate { get; set; } = DateTime.UtcNow;
        //Exprected return date
        public DateTime ExpReturnDate { get; set; }

        // Navigation
        public Book Book { get; set; }

        public BookCopies BookCopy { get; set; }
        public User User { get; set; }
        public override string ToString()
        {
            return $"BorrowTransactionId: {BorrowTransactionsId}, BookId: {BookId}, BookCopyId: {BookCopyId}, UserId: {UserId}, IssueDate: {IssueDate}, ExpReturnDate: {ExpReturnDate}";
        }
        
    }
}