using librarymanagement.ModelLib;
using librarymanagement.DataAccessLib.Contexts;

namespace librarymanagement.DataAccessLib
{
    public class MemberRepository : IMemberRepository
    {
        private readonly LibraryDbContext db;

        public MemberRepository(LibraryDbContext context)
        {
            db = context;
        }
       public bool AddNewUserInDB(Member user)
        {
            var transaction = db.Database.BeginTransaction();
            try
            {
                db.Members.Add(user);
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

        public List<Member> getAllUsersFromDB()
        {
            try
            {
                var users = db.Members.ToList();
                return users;
            }catch(Exception ex)
            {
                Console.WriteLine($"Error to fetch all users: {ex.Message}");
          

                return null;
            }
        }

        public Member getUserByIdFromDB(int userId)
        {
            try
            {
                var user = db.Members.FirstOrDefault(u => u.MemberId == userId);
                return user;
            }catch(Exception ex)
            {
                Console.WriteLine($"Error to fetch all users: {ex.Message}");
                return null;
            }
            
        }

        public Member getUserByContactFromDB(string contact, int contactType)
        {
            try
            {
                if (contactType == 1)
                {
                    var user = db.Members.FirstOrDefault(u => u.Email == contact);
                    return user;
                }
                else
                {
                    var user = db.Members.FirstOrDefault(u => u.PhoneNumber == contact );
                    return user;
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Error to fetch all users: {ex.Message}");
                return null;
            }
        }

        
    }
}