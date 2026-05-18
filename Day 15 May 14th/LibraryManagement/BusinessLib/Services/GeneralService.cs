using System.Data;
using librarymanagementsystem.DataAccessLib;
using librarymanagementsystem.ModelLib;

namespace librarymanagementsystem.BusinessLib
{
    public class GeneralService : IGeneralService
    {
        IGeneralRepository generalRepository;
        IUserManagementService userManagementService;
        public GeneralService()
        {
            generalRepository = new GeneralRepository();
            userManagementService = new UserManagementService();
        }
        public void LibraryRules()
        {
            var rulesList = generalRepository.getAllRules();
            foreach(var rule in rulesList)
            {
                Console.WriteLine(rule.ruleDescription);
            }
            // public static void DisplaySystemRules(
            // decimal finePerDayLateReturn = 10;
            // decimal finePerMissingPageOnReturn = 20;
            // decimal fineOnMissingHardCover = 150;
            // decimal maxPendingFineAmount = 500;
            decimal finePerDayLateReturn = rulesList[0].Value;
            decimal finePerMissingPageOnReturn = rulesList[1].Value;
            decimal fineOnMissingHardCover = rulesList[2].Value;
            decimal maxPendingFineAmount = rulesList[3].Value;
            
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=========================================");
            Console.WriteLine("=========Welcome to VSG Library=========");
            Console.WriteLine("=========================================\n");
            Console.ResetColor();


            // Membership rules
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("MEMBERSHIP RULES:");
            userManagementService.printAllMembershipTypes();
            // Console.WriteLine("Basic Membership   -> Max 2 books, Borrow for 7 days");
            // Console.WriteLine("Student Membership -> Max 3 books, Borrow for 10 days");
            // Console.WriteLine("Premium Membership -> Max 5 books, Borrow for 15 days");
            Console.ResetColor();


            Console.WriteLine();

            // Borrowing rules
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("BORROWING RULES:");

            Console.WriteLine("1. Member account must be active");
            Console.WriteLine("2. Borrowing limit must not be exceeded");
            Console.WriteLine("3. Book copy must be available");
            Console.WriteLine($"4. Pending fine must not exceed Rs. {maxPendingFineAmount}");
            Console.WriteLine("5. Same book cannot be borrowed again until returned");
            Console.ResetColor();


            Console.WriteLine();

            // Fine rules
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("FINE RULES:");

            Console.WriteLine($"1. Late Return Fine          : Rs. {finePerDayLateReturn} per day");
            Console.WriteLine($"2. Missing Page Fine         : Rs. {finePerMissingPageOnReturn} per page");
            Console.WriteLine($"3. Missing Hard Cover Fine   : Rs. {fineOnMissingHardCover}");
            Console.WriteLine($"4. Max Allowed Pending Fine  : Rs. {maxPendingFineAmount}");
            Console.ResetColor();


            Console.WriteLine();

            // Return rules
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("RETURN RULES:");

            Console.WriteLine("1. Returned copy becomes available again");
            Console.WriteLine("2. Book condition is checked during return");
            Console.WriteLine("3. Damage fine is added if pages/cover are missing");
            Console.WriteLine("4. Return transaction is stored in history");
            Console.ResetColor();


            Console.WriteLine();

            // Admin rules
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("ADMIN CONFIGURATION:");

            Console.WriteLine("Admin can modify all fine amounts.");
            Console.WriteLine("Admin can modify max pending fine limit.");
            Console.ResetColor();


            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Press any key to continue...");
            Console.ResetColor();

            Console.ReadKey();

    
        }
        public User UserLogin(string email, string password)
        {
            return generalRepository.UserLogin(email, password);
        }
        public bool AdminLogin(string email, string password)
        {
            return generalRepository.AdminLogin(email, password);
        }
        
    }
}