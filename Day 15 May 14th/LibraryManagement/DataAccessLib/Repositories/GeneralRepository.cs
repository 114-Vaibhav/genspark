using librarymanagementsystem.DataAccessLib.Contexts;
using librarymanagementsystem.ModelLib;

namespace librarymanagementsystem.DataAccessLib
{
    public class GeneralRepository : IGeneralRepository
    {
        LibraryContext db;
        public GeneralRepository( )
        {
            db = new LibraryContext();
        }
        public List<Rules> getAllRules()
        {
            var rules = db.Rules.ToList();
            return rules;
        }
        public User UserLogin(string email, string password)
        {
            try
            {
                var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == password && u.MemberRole != UserRole.Admin);
                if (user != null)
                {
                    return user;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error to fetch user details: {ex.Message}");
                return null;
            }
        }
        public bool AdminLogin(string email, string password)
        {
            try
            {
                var admin = db.Users.FirstOrDefault(a => a.Email == email && a.Password == password && a.MemberRole == UserRole.Admin);
                if (admin != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error to fetch admin details: {ex.Message}");
                return false;
            }
        }
    }
}