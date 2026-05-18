using librarymanagementsystem.DataAccessLib;
namespace librarymanagementsystem.BusinessLib
{
    public class FineService : IFineService
    {
        IFineRepository fineRepository;
        public FineService()
        {
            fineRepository = new FineRepository();
        }
    
        public void PayFine(int userId)
        {
            var pendingFineTransactions = fineRepository.GetPendingFineTransactionsFromDB(userId);
           
            Console.WriteLine("------------------Pending Fine Transactions---------------------");
            foreach (var transaction in pendingFineTransactions)
            {
                Console.WriteLine(transaction);
            }
            Console.WriteLine("------------------------------End------------------------------");
            Console.WriteLine("Enter the ID of the fine transaction you want to pay:");
            if (int.TryParse(Console.ReadLine(), out int fineTransactionId))
            {
                var transactionToPay = pendingFineTransactions.FirstOrDefault(ft => ft.FineTransactionsId == fineTransactionId);
                if (transactionToPay != null)
                {
                    // Here you would implement the logic to mark the transaction as paid in the database
                    Console.WriteLine($"Enter the whole amount to pay for fine transaction ID {fineTransactionId}, your due amount is {transactionToPay.FineAmount}:");
                    if (decimal.TryParse(Console.ReadLine(), out decimal paymentAmount))
                    {
                        Console.WriteLine($"Processing payment for fine transaction ID {fineTransactionId}...");
                        fineRepository.MarkFineTransactionAsPaid(fineTransactionId);
                        Console.WriteLine($"Fine transaction with ID {fineTransactionId} has been marked as paid.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid payment amount. Please try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid fine transaction ID. Please try again.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid fine transaction ID.");
            }
        }
        private void listAllPendingFineTransactions(int userId)
        {
            var pendingFineTransactions = fineRepository.GetPendingFineTransactionsFromDB(userId);
            if (pendingFineTransactions != null && pendingFineTransactions.Count > 0)
            {
                Console.WriteLine("------------------Pending Fine Transactions---------------------");
                foreach (var transaction in pendingFineTransactions)
                {
                    Console.WriteLine(transaction);
                }
                Console.WriteLine("------------------------------End------------------------------");
            }
            else
            {
                Console.WriteLine("No pending fine transactions found.");
            }
        }
        public void ViewPendingFineAmount(int userId)
        {
            try
            {
                decimal pendingFineAmount = fineRepository.GetPendingFineAmountFromDB(userId);
                Console.WriteLine($"Pending Fine Amount: {pendingFineAmount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving fine amount: {ex.Message}");
            }
        }

        public bool HasPendingFineAboveLimit(int userId)
        {
            try
            {
                decimal pendingFineAmount = fineRepository.GetPendingFineAmountFromDB(userId);
                decimal maxPendingFineAmount = fineRepository.GetMaxPendingFineAmountFromDB();
                return pendingFineAmount > maxPendingFineAmount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking pending fine amount: {ex.Message}");
            }
            return false;
        }

        public void ViewFineHistory(int userId)
        {
            try
            {
                var fineTransactions = fineRepository.GetFineTransactionsFromDB(userId);
                if (fineTransactions != null && fineTransactions.Count > 0)
                {
                    Console.WriteLine("------------------Fine History---------------------");
                    foreach (var transaction in fineTransactions)
                    {
                        Console.WriteLine(transaction);
                    }
                }
                else
                {
                    Console.WriteLine("No fine transactions found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving fine history: {ex.Message}");
            }
        }
    }
}