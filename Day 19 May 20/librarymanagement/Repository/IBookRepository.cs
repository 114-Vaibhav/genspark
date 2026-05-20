using librarymanagement.ModelLib;

namespace librarymanagement.DataAccessLib
{
    public interface IBookRepository
    {
        public bool AddNewBookInDB(Book book);
        public List<Book> GetAllBooksFromDB();
        public bool DeleteBookFromDB(int bookId);
        public List<Book> FindBooksFromDB(string searchText, int searchType);
        public Book FindBookById(int bookId);
    }
}