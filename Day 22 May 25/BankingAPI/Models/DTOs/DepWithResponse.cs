namespace BankingAPI.Models.DTOs
{
    public class DepWithResponse
    {
        public string AccountNumber { get; set; } = string.Empty;
        public float Amount { get; set; } = 0;
        public TransactionType transactionType { get; set; } = TransactionType.Deposit;
        public string Status { get; set; } = string.Empty;
        public DateTime transactionDate { get; set; } = DateTime.Now;
        public string message = string.Empty;
    }
}