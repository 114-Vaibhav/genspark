using librarymanagementsystem.ModelLib;

namespace librarymanagementsystem.DataAccessLib
{
    public interface IBorrowRepository
    {
        
        public bool AddBorrowTransactionBookInDB(int userId, int bookId,int bookCopyId, DateTime expReturnDate);
        
        public MembershipTypes GetMembershipDetailsFromDB(int userId);

        public bool CheckActiveBorrowingFromDB(int userId, int bookId);

        public List<BorrowTransactions> GetOverdueBooksFromDB(int userId);

        public List<BorrowTransactions> GetBorrowingHistoryFromDB(int userId);
        
        public List<Book> GetAllBorrowedBooksFromDB();
        public List<BorrowTransactions> GetActiveBorrowingFromDB(int userId);
        public List<Book> GetActiveBorrowingBooksFromDB(int userId);
        public int GetAvailableBookCopyIdFromDB(int bookId);
    }
}