using BankingAPI.Contexts;

using BankingAPI.Models;

namespace BankingAPI.Repositories
{
    public class StatementRepository
    {
        protected  BankingContext _context;
        public StatementRepository(BankingContext context)
        {
            // _context = new BankingContext();
            _context = context;
        }
        public List<AccountStatement> GetAccountStatements(string accountNumber,int pageNo)
        {
            var accountStatements = _context.AccountStatements.Where(x => x.AccountNumber == accountNumber).OrderByDescending(x => x.TransactionDate).Skip((pageNo - 1) * 5).Take(5).ToList();
            return accountStatements;
        }
    }    
}