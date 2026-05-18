using librarymanagementsystem.ModelLib;
using librarymanagementsystem.DataAccessLib.Contexts;

namespace librarymanagementsystem.DataAccessLib
{
    public class ReturnRepository : IReturnRepository
    {
        private LibraryContext db;
        public ReturnRepository()
        {
            db = new LibraryContext();
        }
        public bool AddReturnTransactionBook(int BTid, DateTime returnDate, string conditionOnReturn,int fineAmount = 0, string? remarks = null, int noOfMissingPages = 0, bool isHardCoverMissing = false)
        {
            var transaction = db.Database.BeginTransaction();
            try
            {
                var returnTransaction = new BookReturnTransactions
                {
                    BorrowTransactionId = BTid,
                    ReturnDate = returnDate,
                    ConditionOnReturn = conditionOnReturn,
                    FineAmount = fineAmount,
                    Remarks = remarks,
                    NoOfMissingPages = noOfMissingPages,
                    IsHardCoverMissing = isHardCoverMissing
                };
                db.BookReturnTransactions.Add(returnTransaction);
                db.SaveChanges();
                returnTransaction = db.BookReturnTransactions.Where(bt => bt.BorrowTransactionId == BTid).FirstOrDefault();
                var borrowTransaction = db.BorrowTransactions.Where(bt => bt.BorrowTransactionsId == BTid).FirstOrDefault();
                var bookId = borrowTransaction.BookId;
                var bookCopyId = borrowTransaction.BookCopyId;
                var userId = borrowTransaction.UserId;
                db.Stocks.Where(s => s.BookId == bookId).FirstOrDefault().BorrowedCopies -= 1;
                db.UserStats.Where(us => us.UserId == userId).FirstOrDefault().BooksBorrowed -= 1;
                db.UserStats.Where(us => us.UserId == userId).FirstOrDefault().BooksReturned += 1;
               
                if(conditionOnReturn == "Damaged")
                {
                    db.BookCopies.Where(bc => bc.BookCopiesId == bookCopyId).FirstOrDefault().Status = BookCopyStatus.Damaged;
                    db.Stocks.Where(s => s.BookId == bookId).FirstOrDefault().DamagedCopies += 1;
                
                }
                else if(conditionOnReturn == "DamagedAvailable")
                {                    
                    db.BookCopies.Where(bc => bc.BookCopiesId == bookCopyId).FirstOrDefault().Status = BookCopyStatus.DamagedAvailable;
                    db.Stocks.Where(s => s.BookId == bookId).FirstOrDefault().AvailableCopies += 1;
                }
                else
                {
                    db.BookCopies.Where(bc => bc.BookCopiesId == bookCopyId).FirstOrDefault().Status = BookCopyStatus.Available;
                    db.Stocks.Where(s => s.BookId == bookId).FirstOrDefault().AvailableCopies += 1;
                }

                if(fineAmount > 0)
                {
                    db.UserStats.Where(us => us.UserId == userId).FirstOrDefault().TotalUnpaidFines += fineAmount;
                }

                db.FineTransactions.Add(new FineTransactions
                {
                    BookReturnTransactionId = returnTransaction.BookReturnTransactionsId,
                    UserId = userId,
                    FineAmount = fineAmount,
                    PaymentStatus = PaymentStatus.Unpaid
                });

                
                db.SaveChanges();
                transaction.Commit();
                return true;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding return transaction: {ex.Message}");
                Console.WriteLine($"Error adding return transaction: {ex}");
                return false;
            }
        }

        public List<Rules> GetReturnRulesFromDB()
        {
            try
            {
                var rules = db.Rules.ToList();
                return rules;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving return rules: {ex.Message}");
                return null;
            }
        }
    }
}