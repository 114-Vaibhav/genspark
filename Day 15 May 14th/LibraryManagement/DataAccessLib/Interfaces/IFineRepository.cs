using librarymanagementsystem.ModelLib;

namespace librarymanagementsystem.DataAccessLib
{
    public interface IFineRepository
    {
        public List<FineTransactions> GetFineTransactionsFromDB(int userId);
        public decimal GetPendingFineAmountFromDB(int userId);
        public decimal GetMaxPendingFineAmountFromDB();
        public List<FineTransactions> GetPendingFineTransactionsFromDB(int userId);

        public bool MarkFineTransactionAsPaid(int fineTransactionId);
    }
}