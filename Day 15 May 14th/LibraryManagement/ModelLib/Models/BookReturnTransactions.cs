namespace librarymanagementsystem.ModelLib
{
    public class BookReturnTransactions
    {
        public int BookReturnTransactionsId { get; set; }

        public int BorrowTransactionId { get; set; }

        public DateTime ReturnDate { get; set; } = DateTime.UtcNow;

        public decimal FineAmount { get; set; } = 0;

        public required string ConditionOnReturn { get; set; }
        public int NoOfMissingPages { get; set; } = 0;
        public bool IsHardCoverMissing { get; set; } = false;
        public string? Remarks { get; set; }

        // Navigation
        public BorrowTransactions BorrowTransaction { get; set; }
    }
}