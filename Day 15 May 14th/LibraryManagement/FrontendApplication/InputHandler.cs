using librarymanagementsystem.ModelLib;
using librarymanagementsystem.BusinessLib;
namespace librarymanagementsystem.FronendApplication
{
    public class InputHandler
    {
        public void userCreation()
        {
            UserManagementService userManagementService = new UserManagementService();

            Console.WriteLine("Enter New User's Name: ");
            string name = Console.ReadLine();
            Console.WriteLine("Enter New User's Email: ");
            string email = Console.ReadLine();
            Console.WriteLine("Enter New User's Phone Number: ");
            string phonenumber = Console.ReadLine();
            Console.WriteLine("Enter New User's Password: ");
            string password = Console.ReadLine();

            UserRole userRole;
            Console.WriteLine("Enter New User's Role: ");
            userRole = (UserRole)Enum.Parse(typeof(UserRole), Console.ReadLine());

            Console.WriteLine("Enter New User's Membership Id, Here are the Membership Types: ");
            userManagementService.printAllMembershipTypes();
            int membershipid = Convert.ToInt32(Console.ReadLine());
            userManagementService.AddNewUser(name, email, phonenumber, password, userRole, membershipid);
            
        }
    }
}