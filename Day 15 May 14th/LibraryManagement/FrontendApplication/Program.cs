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
                

                Console.WriteLine("========= LIBRARY SYSTEM =========");
                Console.WriteLine("1. Admin Login");
                Console.WriteLine("2. User Login");
                Console.WriteLine("3. Exit");

                Console.Write("Enter choice: ");
                choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        library.AdminMenu();
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
        }
    }
}