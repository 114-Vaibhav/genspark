using librarymanagementsystem.ModelLib;

namespace librarymanagementsystem.DataAccessLib
{
    public interface IReportRepository
    {
        public List<Book> getOverdueBooksFromDB();
        public (decimal unpaidFinesCollection, decimal paidFinesCollection) getFineCollectionFromDB();
        public List<BookCopies> getCurrentlyBorrowingFromDB();
        public List<(BookCopyStatus status, int count)> getBooksByConditionFromDB();
        public List<IGrouping<string, BookCopies>> getMostBorrowedBooksFromDB();
        public List<IGrouping<string, Book>> getAvailableBooksByCategoryFromDB();
        public List<User> getMemberWithPendingFinesFromDB();


    }
}