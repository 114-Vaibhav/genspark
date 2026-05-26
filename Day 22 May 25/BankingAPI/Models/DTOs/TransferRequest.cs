namespace BankingAPI.Models.DTOs
{
    public class TransferRequest
    {
        public string fromAccountNo { get; set; } = string.Empty;
        public string toAccountNo { get; set; } = string.Empty;
        public float Amount { get; set; } = 0;
        public DateTime transactionDate { get; set; } = DateTime.Now;
    }
}