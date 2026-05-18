using librarymanagementsystem.DataAccessLib.Contexts;
using librarymanagementsystem.ModelLib;
namespace librarymanagementsystem.DataAccessLib
{
    public class FineRepository : IFineRepository
    {
        private LibraryContext db;
        public FineRepository()
        {
            db = new LibraryContext();
        }
        public List<FineTransactions> GetFineTransactionsFromDB(int userId)
        {
            try
            {
                return db.FineTransactions.Where(ft => ft.UserId == userId).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving fine transactions: {ex.Message}");
                return new List<FineTransactions>();
            }
        }
        public decimal GetPendingFineAmountFromDB(int userId)
        {
            try
            {
                return db.UserStats
                    .Where(ft => ft.UserId == userId)
                    .Select(ft => ft.TotalUnpaidFines)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating pending fine amount: {ex.Message}");
                return 0;
            }
        }
        public decimal GetMaxPendingFineAmountFromDB()
        {
            try
            {
                var setting = db.Rules
                    .FirstOrDefault(r => 
                        r.ruleDescription == RuleDescription.MaxPendingFineAmount);

                if (setting != null)
                {
                    return (decimal)setting.Value;
                }
                else
                {
                    Console.WriteLine("MaxPendingFineAmount not found. Using default 500.");
                    return 500;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving MaxPendingFineAmount: {ex.Message}");
                return 500;
            }
        }
        public List<FineTransactions> GetPendingFineTransactionsFromDB(int userId)
        {
            try
            {
                return db.FineTransactions.Where(ft => ft.UserId == userId && ft.PaymentStatus == PaymentStatus.Unpaid).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving pending fine transactions: {ex.Message}");
                return new List<FineTransactions>();
            }
        }
        public bool MarkFineTransactionAsPaid(int fineTransactionId)
        {
            var transaction = db.Database.BeginTransaction();
            try
            {
                var fineTransaction = db.FineTransactions.FirstOrDefault(ft => ft.FineTransactionsId == fineTransactionId);
                if (fineTransaction != null && fineTransaction.PaymentStatus == PaymentStatus.Unpaid)
                {
                    fineTransaction.PaymentStatus = PaymentStatus.Paid;
                    db.FineTransactions.Update(fineTransaction);

                    var userStat = db.UserStats.Where(us => us.UserId == fineTransaction.UserId).FirstOrDefault();
                    if (userStat != null)
                    {
                        userStat.TotalUnpaidFines -= fineTransaction.FineAmount;
                        userStat.TotalFinesPaid += fineTransaction.FineAmount;
                        db.UserStats.Update(userStat);
                    }

                    db.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                else
                {
                    Console.WriteLine($"Fine transaction with ID {fineTransactionId} not found or already paid.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing fine payment: {ex.Message}");
                return false;
            }
        }
    }
}