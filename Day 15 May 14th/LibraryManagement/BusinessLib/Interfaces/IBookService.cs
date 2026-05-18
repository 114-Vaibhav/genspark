using librarymanagementsystem.ModelLib;

namespace librarymanagementsystem.BusinessLib
{
    public interface IBookService
    {
        public void AddNewBook(Book book);

        public void ViewAllBooks();

        public void ViewAllAvailableBooks();

        public void ViewAllDamagedBooks();

        public void ViewAllDamagedAvailableBooks();

        public void FindBook(string searchText, int searchType);

        public void AddCopiesOfBook(int bookId, int numberOfCopies);

        public void UpdateBookCondition(int bookId);

        public void DeleteBook(int bookId);
    }
}