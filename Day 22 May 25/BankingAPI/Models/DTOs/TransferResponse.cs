namespace BankingAPI.Models.DTOs
{
    public class TransferResponse
    {
        public string? fromAccountNo { get; set; } = string.Empty;
        public string? toAccountNo { get; set; } = string.Empty;
        public float sentAmount { get; set; } = 0;
        public float? RemainedBalance { get; set; } = 0;
        public string TransactionReferenceNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime transactionDate { get; set; } = DateTime.Now;
        public string message { get; set; } = string.Empty;
    }
}