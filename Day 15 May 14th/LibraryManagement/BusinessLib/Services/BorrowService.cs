using librarymanagementsystem.ModelLib;
using librarymanagementsystem.DataAccessLib;
namespace librarymanagementsystem.BusinessLib
{
    public class BorrowService : IBorrowService
    {
        IBorrowRepository borrowRepository;
        IReturnRepository returnRepository;
        FineService fineService;
        public BorrowService()
        {
            borrowRepository= new BorrowRepository();
            returnRepository = new ReturnRepository();
            fineService = new FineService();
        }
        public void BorrowBook(int userId, int bookId)
        {   
            Console.WriteLine($"userID {userId} and bookId {bookId}");
            var membershipType = borrowRepository.GetMembershipDetailsFromDB(userId);
            // if(membershipType == null) return;
            int borrowLimit = membershipType.MaxBooksAllowed;
            var activeBorrowing = borrowRepository.GetActiveBorrowingFromDB(userId);
            
           if(CanBorrowBook(userId, bookId) && activeBorrowing.Count < borrowLimit)
            {
                int borrowDurationDays = membershipType.MaxBorrowDays;
                DateTime expReturnDate = DateTime.UtcNow.AddDays(borrowDurationDays);
    
                try
                {
                    var bookCopyId = borrowRepository.GetAvailableBookCopyIdFromDB(bookId);
                    if(borrowRepository.AddBorrowTransactionBookInDB(userId, bookId, bookCopyId, expReturnDate))
                    {
                        Console.WriteLine($"BookCopyId {bookCopyId} of BookId {bookId} Borrowed Successfully");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred while borrowing the book: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Cannot borrow the book. You may have an active borrowing or have reached your borrow limit.");
            }
        }

        public void ReturnBook(int userId, int bookId)
        {
            try
            {
                var activeBorrowing = borrowRepository.GetActiveBorrowingFromDB(userId)
                    .FirstOrDefault(bt => bt.BookId == bookId);

                if (activeBorrowing != null)
                {
                    DateTime returnDate = DateTime.UtcNow;
                    // This can be enhanced to take user input
                    var returnRules = returnRepository.GetReturnRulesFromDB();
                    var (fineAmount, condition,remarks,noOfMissingPages,isHardCoverMissing) = CalculateFineAndCondition(activeBorrowing, returnRules);

                    if(returnRepository.AddReturnTransactionBook(activeBorrowing.BorrowTransactionsId, returnDate, condition, fineAmount, remarks, noOfMissingPages, isHardCoverMissing))
                    {
                        Console.WriteLine("Book returned successfully.");
                    }
                }
                else
                {
                    Console.WriteLine("No active borrowing found for this book and user.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while returning the book: {ex.Message}");
                Console.WriteLine($"Error occurred while returning the book: {ex}");
            }
            
        }
        private (int fine,string condition,string remarks,int NoofMissingPages,bool IsHardCoverMissing) CalculateFineAndCondition(BorrowTransactions borrowTransaction, List<Rules> returnRules)
        {
            int fineAmount = 0;
            DateTime currentDate = DateTime.UtcNow;
            string condition ="Good";
            string remarks = null;
            bool isHardCoverMissing = false;

            if (currentDate > borrowTransaction.ExpReturnDate)
            {
                int daysLate = (currentDate - borrowTransaction.ExpReturnDate).Days;
                fineAmount += daysLate * returnRules[0].Value; 
                remarks = $"Fine {fineAmount} for {daysLate} days late return. ";
            }
            Console.WriteLine("Enter the No of Missing Pages of the book on return : ");
            int missingPages = Convert.ToInt32(Console.ReadLine());
            if (missingPages > 0){
                fineAmount += missingPages * returnRules[1].Value; 
                condition = "Damaged";
                remarks += $"Fine {missingPages * returnRules[1].Value} for missing {missingPages} pages";
            }
            Console.WriteLine("Is the hard cover missing on return? (yes/no): ");
            string hardCoverMissingInput = Console.ReadLine();
            if (hardCoverMissingInput.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                fineAmount += returnRules[2].Value; 
                condition = "DamagedAvailable";
                remarks += $"Fine {returnRules[2].Value} for missing hard cover";
                isHardCoverMissing = true;
            }   
            
            return (fineAmount,condition,remarks,missingPages,isHardCoverMissing);
        }
        public void ViewBorrowedBooks(int userId)
        {
            var borrowedBooks = borrowRepository.GetActiveBorrowingBooksFromDB(userId);
            if (borrowedBooks != null && borrowedBooks.Count > 0)
            {
                Console.WriteLine("------------------Borrowed Books---------------------");
                foreach (var book in borrowedBooks)
                {
                    Console.WriteLine(book);
                }
                Console.WriteLine("---------------------------------------------------");
            }
            else
            {
                Console.WriteLine("No books found for the user.");
            }
        }

        public void ViewOverdueBooks(int userId)
        {
            try
            {
                var overdueBooks = borrowRepository.GetOverdueBooksFromDB(userId);
                if (overdueBooks != null && overdueBooks.Count > 0)
                {
                    Console.WriteLine("------------------Overdue Books---------------------");
                    foreach (var transaction in overdueBooks)
                    {
                        Console.WriteLine(transaction);
                    }
                    Console.WriteLine("---------------------------------------------------");
                }
                else
                {
                    Console.WriteLine("No overdue books found for the user.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching overdue books: {ex.Message}");
            }
        }

        public void ViewBorrowingHistory(int userId)
        {
            try
            {
                var borrowingHistory = borrowRepository.GetBorrowingHistoryFromDB(userId);
                if (borrowingHistory != null && borrowingHistory.Count > 0)
                {
                    Console.WriteLine("------------------Borrowing History---------------------");
                    foreach (var transaction in borrowingHistory)
                    {
                        Console.WriteLine(transaction);
                    }
                    Console.WriteLine("---------------------------------------------------------");
                }
                else
                {
                    Console.WriteLine("No borrowing history found for the user.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching borrowing history: {ex.Message}");
            }
        }

        private bool CanBorrowBook(int userId, int bookId)
        {
            return !HasActiveBorrowing(userId, bookId) && fineService.HasPendingFineAboveLimit(userId) == false;
        }

        private bool HasActiveBorrowing(int userId, int bookId)
        {
            try
            {
                return borrowRepository.CheckActiveBorrowingFromDB(userId, bookId);               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking active borrowing: {ex.Message}");
                return false;
            }
        }

        public void ViewAllBorrowedBooks()
        {
            try
            {
                var borrowedBooks = borrowRepository.GetAllBorrowedBooksFromDB();
                if (borrowedBooks != null && borrowedBooks.Count > 0)
                {
                    Console.WriteLine("------------------All Borrowed Books---------------------");
                    foreach (var book in borrowedBooks)
                    {
                        Console.WriteLine(book);
                    }
                    Console.WriteLine("---------------------------------------------------------");
                }
                else
                {
                    Console.WriteLine("No books are currently borrowed.");
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Error fetching borrowed books: {ex.Message}");
            }
        }
    }
}