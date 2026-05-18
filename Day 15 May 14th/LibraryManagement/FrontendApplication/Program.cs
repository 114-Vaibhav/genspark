using librarymanagementsystem.BusinessLib;

namespace librarymanagementsystem.FronendApplication
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IGeneralService generalService = new GeneralService();
            generalService.LibraryRules();

            Library library = new Library();

            int choice;

            do
            {
                
                Console.ForegroundColor = ConsoleColor.White;
                
                Console.WriteLine("========= LIBRARY SYSTEM =========");
                Console.WriteLine("1. Admin Login");
                Console.WriteLine("2. User Login");
                Console.WriteLine("3. Exit");

                Console.Write("Enter choice: ");
                while (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }

                switch (choice)
                {
                    case 1:
                        library.AdminMenu();
                        // Console.WriteLine("Enter your next ch");
                        break;

                    case 2:
                        library.UserMenu();
                        break;

                    case 3:
                        Console.WriteLine("Goodbye!");
                        break;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }

            } while (choice != 3);
            Console.ResetColor();
        }
    }
}