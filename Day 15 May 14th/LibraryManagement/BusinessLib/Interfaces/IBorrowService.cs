using librarymanagementsystem.ModelLib;

namespace librarymanagementsystem.BusinessLib
{
    public interface IBorrowService
    {
        void BorrowBook(int userId, int bookId);

        void ReturnBook(int userId, int bookId);

        void ViewBorrowedBooks(int userId);

        void ViewOverdueBooks(int userId);

        void ViewBorrowingHistory(int userId);
        void ViewAllBorrowedBooks();
    }
}