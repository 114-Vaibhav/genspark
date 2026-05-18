using librarymanagementsystem.DataAccessLib;
using librarymanagementsystem.ModelLib;

namespace librarymanagementsystem.BusinessLib
{
    public class UserManagementService : IUserManagementService
    {
        IUserRepository userRepository;
        public UserManagementService()
        {
            userRepository = new UserRepository();
        }
        public void AddNewUser(string name, string email, string phonenumber, string password, UserRole userRole, int membershipid)
        {
            User user;
            user = new User();
            try
            {
                userRepository.AddNewUserInDB(user);
                Console.WriteLine("User added successfully.");
            }catch(Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
            }
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

                User user = userRepository.getUserByIdFromDB(userId);
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
                User user = userRepository.getUserByContactFromDB(contact, contactType);
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

        public void UpdateMembershipStatusByAdmin(int userId)
        {
            try
            {   
                int toMembershipId;
                Console.WriteLine("Enter new membership id");
                toMembershipId = Convert.ToInt32(Console.ReadLine());
                if(userRepository.updateMembershipStatusInDB(userId,toMembershipId))
                {
                    
                    Console.WriteLine("Membership status updated successfully");
                    
                }
                else
                {
                    Console.WriteLine("Failed to update membership status");
                    
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
            }
        }

        public void DeactivateUser(int userId)
        {
            try
            {
                if(userRepository.deactivateUserInDB(userId))
                {
                    Console.WriteLine("User deactivated successfully");
                }
                else
                {
                    Console.WriteLine("Failed to deactivate user");
                }
                
            }catch(Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
            }
        }

        public void ViewPersonalDetails(int userId)
        {
            try
            {
                User user = userRepository.getPersonalDetailsFromDB(userId);
                if (user != null)
                {
                    Console.WriteLine("---------------------User Details------------------------");
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
        public void printAllMembershipTypes()
        {
            try
            {
                var membershipTypes = userRepository.getAllMembershipTypesFromDB();
                Console.WriteLine("---------------------Membership Types------------------------");
                foreach (var membershipType in membershipTypes)
                {
                    Console.WriteLine(membershipType);
                }
                Console.WriteLine("---------------------End------------------------");
            }catch(Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
            }
        }
        public void ViewMembershipStatus(int userId)
        {
            try
            {
                MembershipTypes membershipType = userRepository.getMembershipStatusFromDB(userId);
                if (membershipType != null)
                {
                    Console.WriteLine("---------------------Membership Status------------------------");
                    Console.WriteLine(membershipType);
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

        public void DeleteUser(int userId)
        {
            try
            {
                if (userRepository.deleteUserFromDB(userId))
                {
                    Console.WriteLine("User deleted successfully");
                }
                else
                {
                    Console.WriteLine("Failed to delete user");
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
            }
            
        }

    }
}