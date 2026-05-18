namespace librarymanagementsystem.ModelLib
{
    public enum PaymentStatus
    {
        Unpaid,
        Paid
    }

    public class FineTransactions
    {
        public int FineTransactionsId { get; set; }

        public int BookReturnTransactionId { get; set; }

        public int UserId { get; set; }

        public decimal FineAmount { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string? PaymentMethod { get; set; }

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

        public string? PaymentTransactionId { get; set; }

        // Navigation
        public BookReturnTransactions BookReturnTransaction { get; set; }

        public User User { get; set; }
    }
}