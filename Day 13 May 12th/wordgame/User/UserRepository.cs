using Npgsql;
using System.Net.NetworkInformation;

namespace wordgame
{

    public class UserRepository
    {
        string connectionString =
            "Host=localhost;Port=5432;Database=wordgame;Username=vaibhavgupta;Password=8098";
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
                    username TEXT UNIQUE,
                    name TEXT,
                    email TEXT,
                    password TEXT
                );";

            string query = @"
                INSERT INTO users(name, email, username, password) 
                VALUES(@name, @email, @username, @password);";

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
                    command.Parameters.AddWithValue("@username", user.username);
                    command.Parameters.AddWithValue("@password", user.password);

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
        
        public User checkLoginUser(string username, string password)
        {
            string query = @"
                SELECT COUNT(*) FROM users 
                WHERE username = @username AND password = @password;";

            try
            {
                connection.Open();

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);
                    User user=null;
                    
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    if (count > 1)
                    {
                        Console.WriteLine("Multiple users found with the same credentials. Please contact support.");
                        return null;
                    }
                    else if (count == 0)
                    {
                        Console.WriteLine("Invalid username or password.");
                        return null;
                    }
                    else
                    {
                        string getUserQuery = @"
                            SELECT name, email FROM users 
                            WHERE username = @username AND password = @password;";

                        using (var getUserCommand = new NpgsqlCommand(getUserQuery, connection))
                        {
                            getUserCommand.Parameters.AddWithValue("@username", username);
                            getUserCommand.Parameters.AddWithValue("@password", password);

                            using (var reader = getUserCommand.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string name = reader.GetString(0);
                                    string email = reader.GetString(1);
                                    user = new User(name, email, username, password);
                                }
                            }
                        }
                    }
                    return user;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking login: " + ex.Message);
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

    }
}