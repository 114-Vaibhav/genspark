using librarymanagementsystem.ModelLib;
using librarymanagementsystem.BusinessLib;
namespace librarymanagementsystem.FronendApplication
{
    public class InputHandler
    {
        public void userCreation()
        {
            IUserManagementService userManagementService = new UserManagementService();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("Enter New User's Name: ");
            string name = Console.ReadLine().ToLower();
            Console.WriteLine("Enter New User's Email: ");
            string email = Console.ReadLine().ToLower();
            Console.WriteLine("Enter New User's Phone Number: ");
            string phonenumber = Console.ReadLine();
            Console.WriteLine("Enter New User's Password: ");
            string password = ReadPassword();

            UserRole userRole;
            Console.WriteLine("Enter New User's Role(Admin,Basic,Premium,Student): ");
            userRole = (UserRole)Enum.Parse(typeof(UserRole), Console.ReadLine());

            Console.WriteLine("Enter New User's Membership Id, Here are the Membership Types: ");
            userManagementService.printAllMembershipTypes();
            int membershipid = Convert.ToInt32(Console.ReadLine());
            if(userManagementService.AddNewUser(new User(name, email, phonenumber, password, userRole, membershipid)))
            {
                Console.WriteLine("User added successfully.");
            }
            else
            {
                Console.WriteLine("Failed to add user.");
            }
            

            
        }
        public void bookCreation()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("Enter the title of book: ");
            string title = Console.ReadLine().ToLower();
            Console.WriteLine("Enter the author of book: ");
            string author = Console.ReadLine().ToLower();
            Console.WriteLine("Enter the category of book: ");
            string category = Console.ReadLine().ToLower();
            Console.WriteLine("Enter the publication year of book: ");
            string year = Console.ReadLine();
            Console.ResetColor();
            BookService bookService = new BookService();
            bookService.AddNewBook(new Book(title, author, category, year));
        }
        public  string ReadPassword()
        {
            string password = "";

            ConsoleKeyInfo key;

            while (true)
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                    break;

                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[..^1];

                    Console.Write("\b \b");
                    continue;
                }

                password += key.KeyChar;
                Console.Write("*");
            }

            Console.WriteLine();

            return password;
        }
    }
}