using librarymanagementsystem.ModelLib;
using librarymanagementsystem.DataAccessLib.Contexts;

namespace librarymanagementsystem.DataAccessLib
{
    public class BorrowRepository : IBorrowRepository
    {
        private LibraryContext db;
        public BorrowRepository()
        {
            db = new LibraryContext();
        }
        
        public bool AddBorrowTransactionBookInDB(int userId, int bookId,int bookCopyId, DateTime expReturnDate)
        {
            var transaction = db.Database.BeginTransaction();
            try
            {
                var borrowTransaction = new BorrowTransactions
                {
                    UserId = userId,
                    BookId = bookId,
                    BookCopyId = bookCopyId,
                    ExpReturnDate = expReturnDate
                };

                db.BorrowTransactions.Add(borrowTransaction);
                db.BookCopies.Where(bc => bc.BookCopiesId == bookCopyId).FirstOrDefault().Status = BookCopyStatus.Borrowed;
                db.Stocks.Where(s => s.BookId == bookId).FirstOrDefault().BorrowedCopies += 1;
                db.Stocks.Where(s => s.BookId == bookId).FirstOrDefault().AvailableCopies -= 1;
                db.UserStats.Where(us => us.UserId == userId).FirstOrDefault().BooksBorrowed += 1;

                db.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding borrow transaction: {ex.Message}");
                transaction.Rollback();
                return false;
            }
        }

       


        public MembershipTypes GetMembershipDetailsFromDB(int userId)
        {
            try
            {
                var user = db.Users.FirstOrDefault(m => m.UserId == userId);
                return user.MembershipType;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving membership details: {ex.Message}");
                return null;
            }
        }

        public bool CheckActiveBorrowingFromDB(int userId, int bookId)
        {
            try
            {
                var activeBorrowing = db.BorrowTransactions
                    .FirstOrDefault(bt => bt.UserId == userId && bt.BookId == bookId);

                if (activeBorrowing != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking active borrowing: {ex.Message}");
                return false;
            }
        }

        public List<BorrowTransactions> GetOverdueBooksFromDB(int userId)
        {
            try
            {
                var overdueBooks = db.BorrowTransactions
                    .Where(bt => bt.UserId == userId && bt.ExpReturnDate < DateTime.UtcNow)
                    .ToList();

                if (overdueBooks == null || overdueBooks.Count == 0)
                {
                    Console.WriteLine($"No overdue books found for user with ID {userId}.");
                    return new List<BorrowTransactions>();
                }
                else if (overdueBooks.Count > 0)
                {
                    return overdueBooks;
                }
                else
                {
                    throw new Exception("Unexpected error occurred while retrieving overdue books.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving overdue books: {ex.Message}");
                return new List<BorrowTransactions>();
            }
        }

        public List<BorrowTransactions> GetBorrowingHistoryFromDB(int userId)
        {
            try
            {
                var transactions = db.BorrowTransactions
                    .Where(bt => bt.UserId == userId)
                    .ToList();

                if (transactions == null || transactions.Count == 0)
                {
                    Console.WriteLine($"No borrowing history found for user with ID {userId}.");
                    return new List<BorrowTransactions>();
                }
                else if (transactions.Count > 0)
                {
                    return transactions;
                }
                else
                {
                    throw new Exception("Unexpected error occurred while retrieving borrowing history.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving borrowing history: {ex.Message}");
                return new List<BorrowTransactions>();
            }
        }

        public List<Book> GetAllBorrowedBooksFromDB()
        {
            try
            {
                var books = db.Stocks
              .Where(s => s.BorrowedCopies > 0)
              .Select(s => s.Book)
              .Distinct()
              .ToList();

                if (books == null || books.Count == 0)
                {
                    Console.WriteLine("No borrowed books found in the database.");
                    return new List<Book>();
                }
                else if (books.Count > 0)
                {
                    return books;
                }
                else
                {
                    throw new Exception("Unexpected error occurred while retrieving borrowed books.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving borrowed books: {ex.Message}");
                return new List<Book>();
            }
        }
    
        public List<BorrowTransactions> GetActiveBorrowingFromDB(int userId)
        {
            try
            {
                var activeBorrowing = db.BorrowTransactions
                    .Where(bt => bt.UserId == userId && bt.ExpReturnDate >= DateTime.UtcNow)
                    .ToList();

                if (activeBorrowing == null || activeBorrowing.Count == 0)
                {
                    Console.WriteLine($"No active borrowing found for user with ID {userId}.");
                    return new List<BorrowTransactions>();
                }
                else if (activeBorrowing.Count > 0)
                {
                    return activeBorrowing;
                }
                else
                {
                    throw new Exception("Unexpected error occurred while retrieving active borrowing.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving active borrowing: {ex.Message}");
                return new List<BorrowTransactions>();
            }
        }

        public int GetAvailableBookCopyIdFromDB(int bookId)
        {
            try
            {
                var availableCopy = db.BookCopies
                    .Where(s => s.BookId == bookId && s.Status == BookCopyStatus.Available)
                    .FirstOrDefault();

                if (availableCopy == null)
                {
                    throw new Exception("No available book copy found.");
                }

                return availableCopy.BookCopiesId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving available book copy ID: {ex.Message}");
                throw;
            }
        }

    }
}