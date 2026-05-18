using librarymanagementsystem.ModelLib;
using librarymanagementsystem.DataAccessLib.Contexts;

namespace librarymanagementsystem.DataAccessLib
{
    public class UserRepository : IUserRepository
    {
        LibraryContext db = new LibraryContext();
       public bool AddNewUserInDB(User user)
        {
            var transaction = db.Database.BeginTransaction();
            try
            {
                db.Users.Add(user);
                db.UserStats.Add(new UserStat { UserId = user.UserId });
                db.SaveChanges();
                transaction.Commit();
                return true;
            }catch(Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
                Console.WriteLine($"Error  users: {ex}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                return false;
            }
        }

        public List<User> getAllUsersFromDB()
        {
            try
            {
                var users = db.Users.ToList();
                return users;
            }catch(Exception ex)
            {
                Console.WriteLine($"Error to fetch all users: {ex.Message}");
          

                return null;
            }
        }

        public User getUserByIdFromDB(int userId)
        {
            try
            {
                var user = db.Users.FirstOrDefault(u => u.UserId == userId);
                return user;
            }catch(Exception ex)
            {
                Console.WriteLine($"Error to fetch all users: {ex.Message}");
                return null;
            }
            
        }

        public User getUserByContactFromDB(string contact, int contactType)
        {
            try
            {
                if (contactType == 1)
                {
                    var user = db.Users.FirstOrDefault(u => u.Email == contact);
                    return user;
                }
                else
                {
                    var user = db.Users.FirstOrDefault(u => u.PhoneNumber == contact );
                    return user;
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Error to fetch all users: {ex.Message}");
                return null;
            }
        }

        public bool updateMembershipStatusInDB(int userId, int toMembershipId)
        {
            try
            {
                var user = db.Users.FirstOrDefault(u => u.UserId == userId);
                user.MembershipTypesId = toMembershipId;
                user.MembershipType = db.MembershipTypes.FirstOrDefault(m => m.MembershipTypesId == toMembershipId);
                db.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                Console.WriteLine($"Error updating membership status: {ex.Message}");
                return false;
            }
        }

        public bool deactivateUserInDB(int userId)
        {
            try
            {
                var user = db.Users.FirstOrDefault(u => u.UserId == userId);
                user.Status = UserStatus.Inactive;
                db.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                Console.WriteLine($"Error to deactivate user: {ex.Message}");
                return false;
            }
        }

        public User getPersonalDetailsFromDB(int userId)
        {
            try
            {
                var user = db.Users.FirstOrDefault(u => u.UserId == userId);
                return user;
            }catch(Exception ex)
            {
                Console.WriteLine($"Error to fetch personal details of user: {ex.Message}");
                return null;
            }
        }

        public MembershipTypes getMembershipStatusFromDB(int userId)
        {
            try
            {
                var user = db.Users.FirstOrDefault(u => u.UserId == userId);
                return user.MembershipType;
            }catch(Exception ex)
            {
                Console.WriteLine($"Error to fetch all users: {ex.Message}");
                return null;
            }
        }
        public bool deleteUserFromDB(int userId)
        {
            try
            {
                var user = db.Users.FirstOrDefault(u => u.UserId == userId);
                db.Users.Remove(user);
                db.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                Console.WriteLine($"Error to fetch all users: {ex.Message}");
                return false;
            }
        }
        public List<MembershipTypes> getAllMembershipTypesFromDB()
        {
            try
            {
                var membershipTypes = db.MembershipTypes.ToList();
                return membershipTypes;
            }catch(Exception ex)
            {
                Console.WriteLine($"Error to fetch all membership types: {ex.Message}");
                return null;
            }
        }
    }
}