namespace BankingAPI.Models
{
    public class AccountStatement
    {
        public int Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public string AccountNumber { get; set; }
        public float Debit { get; set; }
        public float Credit { get; set; }
        public float Balance { get; set; }

    }
}

