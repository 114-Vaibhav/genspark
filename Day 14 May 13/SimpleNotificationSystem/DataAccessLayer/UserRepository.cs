using Npgsql;
using System.Net.NetworkInformation;
using simplenotification.Contexts;
namespace simplenotification
{
    public class UserRepository
    {
        private NotificationDBContext db;
        public UserRepository()
        {
            db = new NotificationDBContext();        
        }

        public void addUserIntoDatabase(User user)
        {
            try
            {
                using var db = new NotificationDBContext();

                db.Users.Add(user);

                db.SaveChanges();

                Console.WriteLine("User saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving user: " + ex.Message);
            }
        }
        
        public User FindUserByEmail(string email)
        {
            User user=null;
            
            try
            {               
                var userFromDB = db.Users.FirstOrDefault(u => u.email == email);
                if(userFromDB != null){
                    user = userFromDB;
                    System.Console.WriteLine($"User found ");
                    System.Console.WriteLine(user);
                    return user;
                }
            }
            catch (Exception ex)            {
                Console.WriteLine("Error retrieving user: " + ex.Message);
            }
            
            return user;
        }
        public User FindUserByPhone(string phone)
        {
            User user=null;
            
            try
            {               
                var userFromDB = db.Users.FirstOrDefault(u => u.phone == phone);
                if(userFromDB != null){
                    user = userFromDB;
                    System.Console.WriteLine($"User found ");
                    System.Console.WriteLine(user);
                    return user;
                }
            }
            catch (Exception ex)            {
                Console.WriteLine("Error retrieving user: " + ex.Message);
            }
            
             return user;
        }

        public void updateDetails(User user)
        {
            string newname;
            string newemail;
            string newphone;
           
            System.Console.WriteLine("Do you want to update name: type yes or no");
            string inpt = Console.ReadLine() ?? string.Empty;
            if (inpt == "yes")
            {
                System.Console.WriteLine($"Your existing name: {user.name}");
                System.Console.WriteLine("Type your new name: ");
                newname = Console.ReadLine() ?? string.Empty;
            }else
            {
                newname=user.name;
            }
            System.Console.WriteLine("Do you want to update phone: type yes or no");
            inpt = Console.ReadLine() ?? string.Empty;
            if (inpt == "yes")
            {
                System.Console.WriteLine($"Your existing phone: {user.phone}");
                System.Console.WriteLine("Type your new phone: ");
                newphone = Console.ReadLine() ?? string.Empty;
            }else
            {
                newphone=user.phone;
            }
            System.Console.WriteLine("Do you want to update email: type yes or no");
            inpt = Console.ReadLine() ?? string.Empty;
            if (inpt == "yes")
            {
                System.Console.WriteLine($"Your existing email: {user.email}");
                System.Console.WriteLine("Type your new email: ");
                newemail = Console.ReadLine() ?? string.Empty;
            }else
            {
                newemail=user.email;
            }

         
            try
            {
                var userFromDB = db.Users.FirstOrDefault(u => u.UserId == user.UserId);
                if (userFromDB != null)
                {
                    userFromDB.name = newname;
                    userFromDB.email = newemail;
                    userFromDB.phone = newphone;

                    db.SaveChanges();
                }
            } catch(Exception ex)
            {
                System.Console.WriteLine("Error updating user: " + ex.Message);
            }
          
        }

        public void DeleteUserByPhone(string phone)
        {
            
            try
            {
                var userFromDB = db.Users.FirstOrDefault(u => u.phone == phone);
                if(userFromDB != null){
                    db.Users.Remove(userFromDB);
                    db.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                System.Console.WriteLine("Error deleting user: " + ex.Message);
            }
           
            System.Console.WriteLine("User Deleted");
        }
        public void DeleteUserByEmail(string email)
        {
            try
            {
                var userFromDB = db.Users.FirstOrDefault(u => u.email == email);
                if(userFromDB != null){
                    db.Users.Remove(userFromDB);
                    db.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                System.Console.WriteLine("Error deleting user: " + ex.Message);
            }
           
            System.Console.WriteLine("User Deleted");
        }

        private Dictionary<int,User> users = new Dictionary<int, User>();
        public Dictionary<int,User> GetAllUsers()
        {
            
            try
            {
                var usersFromDB = db.Users.ToList();
                foreach(var user in usersFromDB)
                {   
                    System.Console.WriteLine(user);
                    users[user.UserId] = user;
                }
                
            }catch(Exception ex)
            {
                System.Console.WriteLine("Error retrieving users: " + ex.Message);
            }
            return users;
        }

    }
}