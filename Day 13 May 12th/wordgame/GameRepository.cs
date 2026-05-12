using Npgsql;
using System.Net.NetworkInformation;

namespace wordgame
{
    class GameRepository
    {
        string connectionString =
            "Host=localhost;Port=5432;Database=wordgame;Username=vaibhavgupta;Password=8098";
        NpgsqlConnection connection;
        
        public GameRepository()
        {
           connection = new NpgsqlConnection(connectionString);
        }

        public void addGameScoreIntoDatabase(string username, int score,DateTime date,string difficulty)
        {
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS game_scores(
                    id SERIAL PRIMARY KEY,
                    username TEXT,
                    score INT,
                    date DATE,
                    difficulty TEXT
                );";

            string query = @"
                INSERT INTO game_scores(username, score, date, difficulty) 
                VALUES(@username, @score, @date, @difficulty);";

            try
            {
                connection.Open();

                using (var createCommand = new NpgsqlCommand(createTableQuery, connection))
                {
                    createCommand.ExecuteNonQuery();
                }

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@score", score);
                    command.Parameters.AddWithValue("@date", date);
                    command.Parameters.AddWithValue("@difficulty", difficulty);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving game score: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public List<(string username, int score, DateTime date,string difficulty)> GetGameScoresByUsername(string username)
        {
            List<(string username, int score, DateTime date,string difficulty)> scores = new List<(string username, int score, DateTime date,string difficulty)>();

            string query = @"
                SELECT username, score, date, difficulty 
                FROM game_scores 
                WHERE username = @username;";

            try
            {
                connection.Open();

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string user = reader.GetString(0);
                            int score = reader.GetInt32(1);
                            DateTime date = reader.GetDateTime(2);
                            string difficulty = reader.GetString(3);
                            scores.Add((user, score, date, difficulty));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving game scores: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return scores;
        }
        
        public List<(string username, int score, DateTime date, int rank)> GetAllGameScoresWithRank()
        {
            List<(string username, int score, DateTime date, int rank)> scores = new List<(string username, int score, DateTime date, int rank)>();

            string query = @"
                SELECT username, score, date, RANK() OVER (ORDER BY score DESC) as rank 
                FROM game_scores;";

            try
            {
                connection.Open();

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string user = reader.GetString(0);
                            int score = reader.GetInt32(1);
                            DateTime date = reader.GetDateTime(2);
                            int rank = reader.GetInt32(3);
                            scores.Add((user, score, date, rank));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving game scores: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return scores;
        }
    }
}