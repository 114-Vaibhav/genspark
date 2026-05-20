using librarymanagement.DataAccessLib;
using librarymanagement.ModelLib;
using librarymanagement.DataAccessLib.Contexts;
namespace librarymanagement.BusinessLib
{
    public class MemberService : IMemberService
    {
        private readonly MemberRepository userRepository;

        public MemberService(LibraryDbContext context)
        {
            userRepository = new MemberRepository(context);
        }
        // public void AddNewUser(string name, string email, string phonenumber, string password, UserRole userRole, int membershipid)
        public bool AddNewUser(Member user)
        {
            try
            {
                if (ValidateUser(user))
                {
                userRepository.AddNewUserInDB(user);
                return true;
                }
                else
                {
                    return  false;
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Error adding in mgmt user: {ex.Message}");

                return false;
            }
        }
       
        private bool ValidateUser(Member user)
        {
            bool status = true;
            if(user.Name == null || user.Name.Trim().Length < 5)
            {
                Console.WriteLine("Invalid Name, Name should contain more than 4 characters");
                status=false;
        
            }
            if(user.Email == null || !user.Email.Contains("@"))
            {
                Console.WriteLine("Invalid Email");
                status=false;
            }
            if(user.PhoneNumber == null || user.PhoneNumber.Length != 10)
            {
                Console.WriteLine("Invalid Phone Number");
                status=false;
                
            }
            
            return status;
            
        }

        public void ViewAllUsers()
        {
            try
            {
                var users = userRepository.getAllUsersFromDB();
                Console.WriteLine("------------------All Users---------------------");
                foreach (var user in users)
                {
                    Console.WriteLine(user);
                }
                Console.WriteLine("---------------------End------------------------");
            }catch(Exception ex)
            {
                Console.WriteLine($"Error to fetch all users: {ex.Message}");
            }
        }

        public void FindUserById(int userId)
        {
            try
            {   

                Member user = userRepository.getUserByIdFromDB(userId);
                if (user != null)
                {
                    Console.WriteLine("---------------------User Found------------------------");
                    Console.WriteLine(user);
                    Console.WriteLine("---------------------End------------------------");
                }
                else
                {
                    Console.WriteLine("---------------------User Found------------------------");
                    
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
            }
        }

        public void FindUserByContact(string contact, int contactType)
        {
            try
            {   
                Member user = userRepository.getUserByContactFromDB(contact, contactType);
                if (user != null)
                {
                    Console.WriteLine("---------------------User Found------------------------");
                    Console.WriteLine(user);
                    Console.WriteLine("---------------------End------------------------");
                }
                else
                {
                    Console.WriteLine("---------------------User Not Found------------------------");
                    
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
            }
        }

    }
}