using Npgsql;
using System.Net.NetworkInformation;

namespace simplenotification
{
    public class UserRepository
    {
         string connectionString =
            "Host=localhost;Port=5432;Database=simplenotificationsystem;Username=vaibhavgupta;Password=8098";
        NpgsqlConnection connection;
        public UserRepository()
        {
            connection = new NpgsqlConnection(connectionString);
           
        }

        public void addUserIntoDatabase(User user)
        {
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS users(
                    id SERIAL PRIMARY KEY,
                    name TEXT,
                    email TEXT,
                    phone TEXT
                );";

            string query = @"
                INSERT INTO users(name, email, phone) 
                VALUES(@name, @email, @phone);";

            try
            {
                connection.Open();

                using (var createCommand = new NpgsqlCommand(createTableQuery, connection))
                {
                    createCommand.ExecuteNonQuery();
                }

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", user.name);
                    command.Parameters.AddWithValue("@email", user.email);
                    command.Parameters.AddWithValue("@phone", user.phone);

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving user: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        
        public User FindUserByEmail(string email)
        {
            User user=null;
            string query = $"select * from users where email=@Email";
            var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {                connection.Open();
                using (var reader = command.ExecuteReader())                {
                    if (reader.Read())                    {
                        user = new User(
                            reader["name"].ToString(),
                            reader["email"].ToString(),
                            reader["phone"].ToString()
                        );
                        System.Console.WriteLine("----------User Found-------------"); 
                        System.Console.WriteLine(user);
                    }
                    else                    {
                        Console.WriteLine("Email not found");
                    }
                }
            }
            catch (Exception ex)            {
                Console.WriteLine("Error retrieving user: " + ex.Message);
            }
            finally            {
                connection.Close();
             }
            return user;
        }
        public User FindUserByPhone(string phone)
        {
            User user=null;
            string query = $"select * from users where phone=@Phone";
            var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Phone", phone);
            try
            {                connection.Open();
                using (var reader = command.ExecuteReader())                {
                    if (reader.Read())                    {
                         user = new User(
                            reader["name"].ToString(),
                            reader["email"].ToString(),
                            reader["phone"].ToString()
                        );
                        
                        System.Console.WriteLine("----------User Found-------------"); 
                        System.Console.WriteLine(user);
                    }
                    else                    {
                        Console.WriteLine("Phone not found");
                    }
                }
            }
            catch (Exception ex)            {
                Console.WriteLine("Error retrieving user: " + ex.Message);
            }
            finally            {
                connection.Close();
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

            string query = @"
                UPDATE users 
                SET name = @name, email = @email, phone = @phone 
                WHERE email = @oldEmail OR phone = @oldPhone;";
            try
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", newname);
                    command.Parameters.AddWithValue("@email", newemail);
                    command.Parameters.AddWithValue("@phone", newphone);
                    command.Parameters.AddWithValue("@oldEmail", user.email);
                    command.Parameters.AddWithValue("@oldPhone", user.phone);

                    command.ExecuteNonQuery();
                }
                
            } catch(Exception ex)
            {
                System.Console.WriteLine("Error updating user: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
            // User user = new User(id,newname,newemail,newphone);
            // idmapping[newemail]=id;
            // idmapping[newphone]=id;
            // users[id]=user;
        }

        public void DeleteUserByPhone(string phone)
        {
            string query = "DELETE FROM users WHERE phone = @Phone;";
            try
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Phone", phone);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        Console.WriteLine("Phone not found");
                        return;
                    }
                }
            }
            catch(Exception ex)
            {
                System.Console.WriteLine("Error deleting user: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
            System.Console.WriteLine("User Deleted");
        }
        public void DeleteUserByEmail(string email)
        {
             string query = "DELETE FROM users WHERE email = @Email;";
            try
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        Console.WriteLine("Email not found");
                        return;
                    }
                }
            }
            catch(Exception ex)
            {
                System.Console.WriteLine("Error deleting user: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
            System.Console.WriteLine("User Deleted");
        }

        private Dictionary<int,User> users = new Dictionary<int, User>();
        public Dictionary<int,User> GetAllUsers()
        {
            string query = "select * from users";
            var command = new NpgsqlCommand(query, connection);
            try
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())                {
                    User user = new User(
                        reader["name"].ToString(),
                        reader["email"].ToString(),
                        reader["phone"].ToString()
                    );
                    users.Add(reader.GetInt32(0), user);
                }
                
            }catch(Exception ex)
            {
                System.Console.WriteLine("Error retrieving users: " + ex.Message);
            }finally
            {
                connection.Close();
            }
            return users;
        }

    }
}