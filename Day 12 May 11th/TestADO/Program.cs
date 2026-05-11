using Npgsql;
using System.Net.NetworkInformation;

namespace UnderstandingADOApp
{
    
    internal class Program
    {
        string connectionString =
            "Host=localhost;Port=5432;Database=dummydb;Username=postgres;Password=password";
        NpgsqlConnection connection;
        public Program()
        {
          connection = new NpgsqlConnection(connectionString);
           
        }
        void GetProductDataFromDatabase()
        {
            string selectQuery = "Select * from Products";
            NpgsqlCommand command = new NpgsqlCommand(selectQuery, connection);
            try
            {
                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("Product Id : " + reader[0].ToString());
                    Console.WriteLine("Product Name : " + reader[1].ToString());
                }
                Console.WriteLine("Done reading");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection?.Close();
            }

        }

        void InsertUserInToDatabase()
        {
            User user = GetUserDataFromConsole();
            string insertCmd = $"Insert into Users values('{user.Username}','{user.Password}','{user.Role}')";
            NpgsqlCommand command = new NpgsqlCommand(insertCmd, connection);
            try
            {
                connection.Open();
                int result = command.ExecuteNonQuery();
                if(result>0)
                    Console.WriteLine("User created successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                connection?.Close();
            }
        }
        void UpdateUserToDatabase(User user, string field)
        {
            string updateCmd = "";
            if (field == "password")
                updateCmd = $"update  Users set user_password = '{user.Password}' where user_id='{user.Username}'";
            else if (field == "role")
                updateCmd = $"update  Users set user_role = '{user.Role}' where user_id='{user.Username}'";
            else
                throw new Exception("Invalid column to update");

            NpgsqlCommand command = new NpgsqlCommand(updateCmd, connection);
            try
            {
                connection.Open();
                int result = command.ExecuteNonQuery();
                if (result > 0)
                    Console.WriteLine("User details updated");
                else
                    Console.WriteLine("No such user");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                connection?.Close();
            }
        }
        void InitiateUserDataUpdate()
        {
            Console.WriteLine("Please enter the user id you want to update");
            User user = new User();
            user.Username = Console.ReadLine() ?? "";
            Console.WriteLine("Please enter the field you want to update Password/Role");
            
            string option = Console.ReadLine()??"".ToLower();
            if(option =="role")
            {
                Console.WriteLine("Please enter the new value for role");
                user.Role = Console.ReadLine() ?? "";
            }
            else if(option == "password")
            {
                Console.WriteLine("Please enter the new value for password");
                user.Password = Console.ReadLine() ?? "";
            }
            try
            {
                UpdateUserToDatabase(user, option);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        private User GetUserDataFromConsole()
        {
            User user = new User();
            Console.WriteLine("Please eneter your preffered username");
            user.Username = Console.ReadLine()??"";
            Console.WriteLine("Please eneter teh password");
            user.Password = Console.ReadLine()??"";
            Console.WriteLine("Please eneter your role");
            user.Role = Console.ReadLine() ?? "";
            return user;

        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            new Program().InitiateUserDataUpdate();

        }
    }
    public class User
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}

// using Npgsql;
// using System.Net.NetworkInformation;

// namespace UnderstandingADOApp
// {
    
//     internal class Program
//     {
//          string connectionString =
//             "Host=localhost;Port=5432;Database=dummydb;Username=vaibhavgupta;Password=8098";
//         NpgsqlConnection connection;
//         public Program()
//         {
//           connection = new NpgsqlConnection(connectionString);
           
//         }
//         void GetProductDataFromDatabase()
//         {
//             string selectQuery = "Select * from Products";
//             NpgsqlCommand command = new NpgsqlCommand(selectQuery, connection);
//             try
//             {
//                 connection.Open();
//                 NpgsqlDataReader reader = command.ExecuteReader();
//                 while (reader.Read())
//                 {
//                     Console.WriteLine("Product Id : " + reader[0].ToString());
//                     Console.WriteLine("Product Name : " + reader[1].ToString());
//                 }
//                 Console.WriteLine("Done reading");
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine(ex.Message);
//             }
//             finally
//             {
//                 connection?.Close();
//             }

//         }
        
//         void UpdatetUserPasswordIntoDatabase()
//         {   
//             string newpassword;
//             string username;
//             System.Console.WriteLine("Enter Username: ");
//             username = Console.ReadLine();
//             System.Console.WriteLine("Enter new Password: ");
//             newpassword = Console.ReadLine();
//             string updateQuery = $"UPDATE Users SET Password = '{newpassword}' WHERE Username = '{username}'";            
//             NpgsqlCommand command = new NpgsqlCommand(updateQuery, connection);
            
//             try
//             {
//                 connection.Open();
//                 int result = command.ExecuteNonQuery();
//                 if (result > 0)
//                 {
//                     System.Console.WriteLine("User Password updated");
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine(ex.Message);
//             }
//             finally
//             {
//                 connection?.Close();
//             }

//         }
        
//         void InsertUserDataIntoDatabase()
//         {   
//             User user = GetUserDataFromConsole();
//             string insertQuery = $"Insert into Users values('{user.Username}','{user.Password}','{user.Role}')";
//             NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection);
            
//             try
//             {
//                 connection.Open();
//                 int result = command.ExecuteNonQuery();
//                 if (result > 0)
//                 {
//                     System.Console.WriteLine("User Created");
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine(ex.Message);
//             }
//             finally
//             {
//                 connection?.Close();
//             }

//         }
        

//        private User GetUserDataFromConsole()
//         {
//             User user = new User();
//             System.Console.WriteLine("Enter your username: ");
//             user.Username = Console.ReadLine();
//             System.Console.WriteLine("Enter your Password: ");
//             user.Password = Console.ReadLine();
//             System.Console.WriteLine("Enter your Role: ");
//             user.Role = Console.ReadLine();

//             return user;
//         }
//         static void Main(string[] args)
//         {
//             // Console.WriteLine("Hello, World!");
//             // new Program().GetProductDataFromDatabase();
//             // new Program().InsertUserDataIntoDatabase();
//             new Program().UpdatetUserPasswordIntoDatabase();
//         }
//     }
//     public class User
//     {
//         public string Username {get;set;} = string.Empty;
//         public string Password {get;set;}= string.Empty;
//         public string Role {get;set;}= string.Empty;

//     }
// }
