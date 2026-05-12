
namespace wordgame
{

    public class UserServices
    {
        // stri
        // ost=localhost;Port=5432;Database=wordgame;Username=vaibhavgupta;Password=8098";
        // NpgsqlConnection connection;
        UserRepository userRepository;
        public UserServices()
        {
            userRepository = new UserRepository();
           
        }
      
        private bool IsValidEmail(string email)
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(email);
                    return addr.Address == email;
                }
                catch
                {
                    return false;
                }
            }
    
        public User CreateUser()
        {
            System.Console.WriteLine("Enter the user's name: ");
            string name = Console.ReadLine() ?? string.Empty;
            System.Console.WriteLine("Enter the email id: ");
            
            string email;
            while (true)
            {
                email = Console.ReadLine() ?? string.Empty;
                if (IsValidEmail(email))
                {
                    break;
                }

                Console.WriteLine("Invalid, try again");
            }
            string username;
            System.Console.WriteLine("Enter the username: ");
            while (true)
            {
                username = Console.ReadLine() ?? string.Empty;
                if (!string.IsNullOrEmpty(username))
                {
                    break;
                }

                Console.WriteLine("Username cannot be empty, try again");
            }
            string password;
            System.Console.WriteLine("Enter the password: ");
            while (true)
            {
                password = Console.ReadLine() ?? string.Empty;
                if (!string.IsNullOrEmpty(password))
                {
                    break;
                }

                Console.WriteLine("Password cannot be empty, try again");
            }

            User user = new User(name,email,username,password);
            userRepository.addUserIntoDatabase(user);
            return user;
        } 
  
        public User loginUser()
        {
            System.Console.WriteLine("Enter the username: ");
            string username = Console.ReadLine() ?? string.Empty;
            System.Console.WriteLine("Enter the password: ");
            string password = Console.ReadLine() ?? string.Empty;
            
            User user = userRepository.checkLoginUser(username, password);
            return user;
        }
      


    }
}