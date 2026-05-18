using System.Data.Common;
using librarymanagementsystem.DataAccessLib.Contexts;
using librarymanagementsystem.ModelLib;

namespace librarymanagementsystem.DataAccessLib
{
    public class ReportRepository: IReportRepository
    {
        LibraryContext db;
        public ReportRepository()
        {
            db = new LibraryContext();
        }
        public List<Book> getOverdueBooksFromDB()
        {
            try
            {
                var overduesboks = db.BorrowTransactions.Where(bt => bt.ExpReturnDate < DateTime.UtcNow).Select(bt => bt.Book).ToList();
                return overduesboks;
            }catch(Exception ex)
            {
                Console.WriteLine($"Error generating overdue books report: {ex.Message}");
                return null;
            }
        }
        public (decimal unpaidFinesCollection, decimal paidFinesCollection) getFineCollectionFromDB()
        {
            try
            {
                var unpaidFinesCollection = db.UserStats.Where(u => u.TotalUnpaidFines > 0).Select(u => u.TotalUnpaidFines).Sum();
                var paidFinesCollection = db.UserStats.Where(u => u.TotalUnpaidFines > 0).Select(u => u.TotalFinesPaid).Sum();
                return (unpaidFinesCollection, paidFinesCollection);
                
            }catch(Exception ex)
            {
                Console.WriteLine($"Error generating fine collection report: {ex.Message}");
                return (0,0);
            }
            
        }
        
        public List<BookCopies> getCurrentlyBorrowingFromDB()
        {
            try
            {
                var borrowReport = db.BookCopies.Where(bc => bc.Status == BookCopyStatus.Borrowed).ToList();
                return borrowReport;
            }catch (Exception ex)
            {
                Console.WriteLine($"Error generating currently borrowing report: {ex.Message}");
                return null;
            }
        }
        public List<(BookCopyStatus status, int count)> getBooksByConditionFromDB()
        {
            try
            {
                var books = db.BookCopies
                    .GroupBy(bc => bc.Status)
                    .Select(g => new
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .AsEnumerable()   
                    .Select(x => (x.Status, x.Count))
                    .ToList();

                return books;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating books by condition report: {ex.Message}");
                return new List<(BookCopyStatus, int)>();
            }
        }
        public List<User> getMemberWithPendingFinesFromDB()
        {
            try
            {
                var users = db.UserStats.Where(u => u.TotalUnpaidFines > 0).Select(u => u.User).ToList();
                return users;
            }catch (Exception ex)
            {
                Console.WriteLine($"Error generating member with pending fines report: {ex.Message}");
                return null;
            }
            
        }
        public List<IGrouping<string, BookCopies>> getMostBorrowedBooksFromDB()
        {
            try
            {
                var mostBorrowed = db.BookCopies.Where(bc => bc.Status == BookCopyStatus.Available).GroupBy(bc => bc.Book.Title).OrderByDescending(g => g.Count()).ToList();
                return mostBorrowed;
                
            }catch (Exception ex)
            {
                Console.WriteLine($"Error generating most borrowed books report: {ex.Message}");
                return null;
            }
            
        }
        
        
        public List<IGrouping<string, Book>> getAvailableBooksByCategoryFromDB()
        {
            try
            {
                var avlBooks = db.BookCopies.Where(bc => bc.Status == BookCopyStatus.Available).Select(bc => bc.Book).GroupBy(b => b.Category).ToList();
                if(avlBooks == null || avlBooks.Count == 0)
                {
                    Console.WriteLine("No available books found in the database.");
                    return new List<IGrouping<string, Book>>();
                }else if(avlBooks.Count > 0)
                {
                   return avlBooks;
                }else {
                    throw new Exception("Unexpected error occurred while retrieving available books.");
                    
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error retrieving available books: {ex.Message}");
                return new List<IGrouping<string, Book>>();
            }
            
        }


    }
}