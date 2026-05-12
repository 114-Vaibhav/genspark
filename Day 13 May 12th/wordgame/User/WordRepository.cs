using Npgsql;
using System.Net.NetworkInformation;

namespace wordgame
{
    class WordRepository
    {
        string connectionString =
            "Host=localhost;Port=5432;Database=wordgame;Username=vaibhavgupta;Password=8098";
        NpgsqlConnection connection;
        
        public WordRepository()
        {
           connection = new NpgsqlConnection(connectionString);
        }
        public void addWordIntoDatabase(string word, string level)
        {
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS words(
                    id SERIAL PRIMARY KEY,
                    word TEXT UNIQUE,
                    level TEXT
                );";

            string query = @"
                INSERT INTO words(word, level) 
                VALUES(@word, @level);";

            try
            {
                connection.Open();

                using (var createCommand = new NpgsqlCommand(createTableQuery, connection))
                {
                    createCommand.ExecuteNonQuery();
                }

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@word", word);
                    command.Parameters.AddWithValue("@level", level);

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving word: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        
        public string getWordFromDatabase(string level)
        {
            string query = @"
                SELECT word FROM words
                WHERE level = @level
                ORDER BY RANDOM()
                LIMIT 1;";

            try
            {
                connection.Open();

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@level", level);
                    // Console.WriteLine("REsultis: "+command.ExecuteScalar());
                    var result = command.ExecuteScalar();
                    return result != null ? result.ToString() : null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving word: " + ex.Message);
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

    }
}