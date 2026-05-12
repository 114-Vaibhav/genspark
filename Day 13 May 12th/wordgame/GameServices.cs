namespace wordgame
{
    class GameServices
    {
        
        GameRepository gameRepository;
        UserServices userServices;
        private string username;
        Game game;
        HashSet<string> loggedusers;
        public GameServices()
        {
            username="";
            gameRepository = new GameRepository();
            userServices = new UserServices();
            game = new Game();
            loggedusers= new HashSet<string>();
        }
        public void ShowRules()
        {

            System.Console.ForegroundColor = ConsoleColor.Cyan;
            System.Console.WriteLine("Game Rules:\n");

            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("========== WORD GUESS GAME RULES ==========");
            System.Console.WriteLine();

            System.Console.WriteLine("Feedback Rules:");
            System.Console.WriteLine("G = Correct letter in correct position");
            System.Console.WriteLine("Y = Correct letter in wrong position");
            System.Console.WriteLine("X = Letter not present in the word");

            System.Console.WriteLine();
            System.Console.WriteLine("Sample Feedback:");
            System.Console.WriteLine("Hidden Word: MANGO");
            System.Console.WriteLine("User Guess : MAGIC");
            System.Console.WriteLine("Output:");
            System.Console.WriteLine("M A G I C");
            System.Console.WriteLine("G G Y X X");

            System.Console.WriteLine();
            System.Console.WriteLine("Game Rules:");
            System.Console.WriteLine("1. You have a maximum of 6 attempts to guess the word.");
            System.Console.WriteLine("2. Enter only one word per attempt.");
            System.Console.WriteLine("3. Input must contain only English letters (A-Z).");
            System.Console.WriteLine("4. No numbers, spaces, or special characters are allowed.");
            System.Console.WriteLine("5. Word comparison is case-insensitive.");
            System.Console.WriteLine("6. If you guess the correct word, the game ends immediately.");
            System.Console.WriteLine("7. If all 6 attempts are used, the game is over.");
            System.Console.WriteLine("8. Feedback contains only X, G, and Y:");
            System.Console.WriteLine("   X = Letter not present in the word");
            System.Console.WriteLine("   G = Correct letter in correct position");
            System.Console.WriteLine("   Y = Correct letter in wrong position");
            System.Console.WriteLine("9. Score Formula = (7 - Attempts Used) × Level");
            System.Console.WriteLine("   Easy = 1, Medium = 2, Hard = 3");
            System.Console.WriteLine("   Example: If you win in 2 attempts on Hard → (7-2)×3 = 15");

            System.Console.WriteLine("===========================================");
            System.Console.ForegroundColor = ConsoleColor.Magenta;
            System.Console.WriteLine("\nDifficulty Levels:\n");

            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("Easy   -> 3-letter words");

            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("Medium -> 4-letter words");

            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("Hard   -> 5-letter words");

            System.Console.ForegroundColor = ConsoleColor.Cyan;
            System.Console.WriteLine("\nTip: Choose your difficulty wisely!");

            System.Console.ResetColor();
            System.Console.WriteLine("\nPress any key to start...");
            System.Console.ReadKey();
        }

        public void playGame()
        {   
            if(username=="")
            {
                Console.WriteLine("Please login to play the game.");
                return;
            }
            var score = game.playGame();
            gameRepository.addGameScoreIntoDatabase(username,score,DateTime.Now,game.difficulty);
        }  
        public void SignUp()
        {
            if(loggedusers.Count>=1)
            {
                Console.WriteLine("Maximum number of logged in users reached. Please Try again");
                loggedusers.Clear();
                username="";
                return;
            }
            userServices.CreateUser();
        }
        public void Login()
        {
            if(loggedusers.Count>=1)
            {
                Console.WriteLine("Maximum number of logged in users reached. Please Try again");
                loggedusers.Clear();
                username="";
                return;
            }
            User user = userServices.loginUser();
            if (user != null)
            {
                loggedusers.Add(user.username);
                username = user.username;
                Console.WriteLine("Login successful! Welcome, " + user.username);
            }
            else
            {
                Console.WriteLine("Login failed. Please check your credentials and try again.");
            }

        }

        public void ListScores()
        {
            if(username=="")
            {
                Console.WriteLine("Please login to view your scores.");
                return;
            }
            var scores = gameRepository.GetGameScoresByUsername(username);
            if (scores.Count == 0)
            {
                Console.WriteLine("No scores found for user: " + username);
            }
            else
            {
                Console.WriteLine($"--------------------------- Scores for user: {username} ----------------");
                // Console.WriteLine($"Scores for user: {username}");
                foreach (var score in scores)
                {
                    Console.WriteLine($"Score: {score.score}, Date: {score.date}, Difficulty: {score.difficulty}");
                }
            }
        }
        public void ListUsersWithRank()
        {
            var userRanks = gameRepository.GetAllGameScoresWithRank();
            if (userRanks.Count == 0)
            {
                Console.WriteLine("No user scores found.");
            }
            else
            {
                Console.WriteLine("--------------------------- User Ranks ----------------");
                Console.WriteLine("Rank | Username | Total Score");
                int rank = 1;
                foreach (var user in userRanks)
                {
                    Console.WriteLine($"{user.rank}    | {user.username} | {user.score}");
                    rank++;
                }
            }
        }

    }
}