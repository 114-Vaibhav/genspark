using librarymanagement.ModelLib;

namespace librarymanagement.BusinessLib
{
    public interface IBookService
    {
        public void AddNewBook(Book book);

        public void ViewAllBooks();
        public void FindBook(string searchText, int searchType);

        public void DeleteBook(int bookId);
    }
}