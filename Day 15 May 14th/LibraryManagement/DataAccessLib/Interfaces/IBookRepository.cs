using librarymanagementsystem.ModelLib;

namespace librarymanagementsystem.DataAccessLib
{
    public interface IBookRepository
    {
        public bool AddNewBookInDB(Book book);

        public List<Book> GetAllBooksFromDB();

        public List<Book> GetAllAvailableBooksFromDB();

        public List<Book> GetAllDamagedBooksFromDB();

        public List<Book> GetAllDamagedAvailableBooksFromDB();

        public Book? FindBookFromDB(string searchText, int searchType);

        public bool AddCopyOfBookInDB(int bookId);

        public bool UpdateBookConditionInDB(int bookCopyId, int damagePercentage, string damageDescription);

        public bool DeleteBookFromDB(int bookId);
    }
}