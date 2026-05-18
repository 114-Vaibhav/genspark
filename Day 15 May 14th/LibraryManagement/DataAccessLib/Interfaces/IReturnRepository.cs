using librarymanagementsystem.ModelLib;

namespace librarymanagementsystem.DataAccessLib
{
    public interface IReturnRepository
    {
        
        public bool AddReturnTransactionBook(int BTid, DateTime returnDate, string conditionOnReturn,int fineAmount = 0, string? remarks = null, int noOfMissingPages = 0, bool isHardCoverMissing = false);
        public List<Rules> GetReturnRulesFromDB();   
    }
}