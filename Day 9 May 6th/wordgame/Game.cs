namespace wordgame
{
    public class Game
    {
        private int attempt;
        private Dictionary<int,string>comment;
        public Game()
        {
            attempt=0;
            comment= new Dictionary<int, string>
            {
                {1, "Genius!"},
                {2, "Excellent!"},
                {3, "Great job!"},
                {4, "Good work!"},
                {5, "Nice try!"},
                {6, "That was close!"}
            };
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
            System.Console.WriteLine("Enter your difficulty level 1: Easy 2:Medium 3:Hard ");
            int level;
            while (!int.TryParse(Console.ReadLine(), out level) || level < 1 || level > 3)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Please enter only 1, 2, or 3.");
                Console.ResetColor();
            }
            string answer;
            int size;
            if (level == 1)
            {
                answer = new WordProvider().getWord("easy");
                size=3;
            }else if (level == 2)
            {
                answer = new WordProvider().getWord("medium");
                size=4;
            }
            else
            {
                answer = new WordProvider().getWord("hard");
                size=5;
            }
            HashSet<string> previousGuesses = new HashSet<string>();
            attempt=1;
            while (attempt <= 6)
            {
                System.Console.Write($"\n----------Attempt {attempt}: Enter your word ");
                string guess = Console.ReadLine();

                try
                {
                    if (new GuessValidator().ValidateGuess(guess, size))
                    {
                        guess = guess.ToUpper();

                        guess = guess.ToUpper();

                        if (previousGuesses.Contains(guess))
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("You already guessed this word.");
                            Console.ResetColor();
                            continue;
                        }

                        previousGuesses.Add(guess);
                        
                        string feedback = new FeedbackGenerator().generateFeedback(guess, answer, size);

                        if (feedback == new string('G', size))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Congratulations! You guessed. {comment[attempt]}");
                            Console.ResetColor();
                            return;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Try again. Here is feedback: {feedback}");
                            Console.ResetColor();

                        }

                        if (attempt == 6)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"\n\n\nGame Over! Word was: {answer}\n\n");
                            Console.ResetColor();
                        }
                    }
                }
                catch (InvalidGuessException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();

                    continue; 
                }
            attempt++;
            }
        }
    }
}