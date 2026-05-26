using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankingAPI.Models.DTOs
{
    public class ACStatementRequest
    {
        public string AccountNumber { get; set; } = string.Empty;
        public int pageNo { get; set; } = 1;
    }
}