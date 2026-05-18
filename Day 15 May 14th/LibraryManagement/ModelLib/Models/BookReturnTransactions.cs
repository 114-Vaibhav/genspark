namespace librarymanagementsystem.ModelLib
{
    public class BookReturnTransactions
    {
        public int BookReturnTransactionsId { get; set; }

        public int BorrowTransactionId { get; set; }

        public DateTime ReturnDate { get; set; } = DateTime.UtcNow;

        public decimal FineAmount { get; set; } = 0;

        public string ConditionOnReturn { get; set; }
        public int NoOfMissingPages { get; set; } = 0;
        public bool IsHardCoverMissing { get; set; } = false;
        public string? Remarks { get; set; }

        // Navigation
        public BorrowTransactions BorrowTransaction { get; set; }


        public override string ToString()
        {
            return $"BookReturnTransactionId: {BookReturnTransactionsId}, "+
            $"BorrowTransactionId: {BorrowTransactionId}, "+
            $"ReturnDate: {ReturnDate}, FineAmount: {FineAmount}, "+
            $"ConditionOnReturn: {ConditionOnReturn}, NoOfMissingPages: {NoOfMissingPages}, "+
            $"IsHardCoverMissing: {IsHardCoverMissing}, Remarks: {Remarks} ";
            // $"BookId: {BorrowTransaction.BookId}, BookCopyId: {BorrowTransaction.BookCopyId}, UserId: {BorrowTransaction.UserId}, IssueDate: {BorrowTransaction.IssueDate}";
        }
    }
}