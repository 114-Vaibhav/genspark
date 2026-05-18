using librarymanagementsystem.ModelLib;
using librarymanagementsystem.DataAccessLib;

namespace librarymanagementsystem.BusinessLib
{
    public class ReportsService : IReportsService
    {
        IReportRepository reportRepository;
        public ReportsService()
        {
            reportRepository = new ReportRepository();
        }
        public void generateOverdueBooksReport()
        {
            try
            {
                var overdueBooks = reportRepository.getOverdueBooksFromDB();
                if (overdueBooks != null && overdueBooks.Count > 0)
                {
                    Console.WriteLine("------------------Overdue Books---------------------");
                    foreach (var book in overdueBooks)
                    {
                        Console.WriteLine(book);
                    }
                    Console.WriteLine("---------------------------------------------------");
                }
                else
                {
                    Console.WriteLine("No overdue books found.");
                }
                
            }catch(Exception ex)
            {
                Console.WriteLine($"Error generating overdue books report: {ex.Message}");
            }
        }

        public void generateFineCollectionReport()
        {
            (decimal unpaidFinesCollection, decimal paidFinesCollection) = reportRepository.getFineCollectionFromDB();
            Console.WriteLine($"Unpaid Fines Collection: {unpaidFinesCollection}");
            Console.WriteLine($"Paid Fines Collection: {paidFinesCollection}");
        }

        public void generateCurrentlyBorrowingReport()
        {
            var currentlyBorrowing = reportRepository.getCurrentlyBorrowingFromDB();
            Console.WriteLine("------------------Currently Borrowing---------------------");
            foreach (var book in currentlyBorrowing)
            {
                Console.WriteLine(book);
            }
            Console.WriteLine("---------------------------------------------------");
        }

        public void generateBookConditionReport()
        {
            var booksByCondition = reportRepository.getBooksByConditionFromDB();
            Console.WriteLine("------------------Books Count With Condition---------------------");
            foreach ( (BookCopyStatus status, int count) in booksByCondition)
            {
                Console.WriteLine($"Status: {status}, Count: {count}");
            }
            Console.WriteLine("---------------------------------------------------");
        }

        public void generateMemberWithPendingFinesReport()
        {
            var membersWithPendingFines = reportRepository.getMemberWithPendingFinesFromDB();
            Console.WriteLine("------------------Members With Pending Fines---------------------");
            foreach (var member in membersWithPendingFines)
            {
                Console.WriteLine(member);
            }
            Console.WriteLine("---------------------------------------------------");
        }


        public void generateMostBorrowedBooksReport()
        {
            var mostBorrowedBooks = reportRepository.getMostBorrowedBooksFromDB();

            Console.WriteLine("------------------Most Borrowed Books---------------------");

            // Null check
            if (mostBorrowedBooks == null || !mostBorrowedBooks.Any())
            {
                Console.WriteLine("No borrowed books found.");
                Console.WriteLine("---------------------------------------------------");
                return;
            }

            foreach (var group in mostBorrowedBooks)
            {
                if (group == null)
                    continue;

                Console.WriteLine($"Book: {group.Key ?? "Unknown"}");

                var count = group.Count();
                Console.WriteLine($"Count: {count}");

                foreach (var bookCopy in group)
                {
                    if (bookCopy != null)
                    {
                        Console.WriteLine(bookCopy);
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine("---------------------------------------------------");
        }
        public void generateAvailableBooksByCategoryReport()
        {
            var availableBooksByCategory = reportRepository.getAvailableBooksByCategoryFromDB();
            Console.WriteLine("------------------Available Books By Category---------------------");
            foreach (var group in availableBooksByCategory)
            {
                Console.WriteLine($"Category: {group.Key}");
                Console.WriteLine($"Count: {group.Count()}");

                foreach (var book in group)
                {
                    Console.WriteLine(book);
                }
            }
            Console.WriteLine("---------------------------------------------------");
        }

        

    }
}