namespace BankingAPI.Models.DTOs
{
    public enum TransactionType{
        Deposit,
        Withdraw
    }
    public class DepWithRequest
    {
        public string AccountNumber { get; set; } = string.Empty;
        public float Amount { get; set; } = 0;
        public TransactionType transactionType { get; set; }
        public DateTime transactionDate { get; set; } = DateTime.Now;
    }
}