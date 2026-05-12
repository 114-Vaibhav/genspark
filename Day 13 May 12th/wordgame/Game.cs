namespace wordgame
{
    public class Game
    {
        private int attempt;
        
        public string difficulty;
        public int score;
        private Dictionary<int,string>comment;
        public Game()
        {
            attempt=0;
            score=0;
            difficulty="";
            
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
       
        public int playGame()
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
                answer = new WordProvider().getWord("Easy");
                difficulty="Easy";
                size=3;
            }else if (level == 2)
            {
                answer = new WordProvider().getWord("Medium");
                difficulty="Medium";
                size=4;
            }
            else
            {
                answer = new WordProvider().getWord("Hard");
                difficulty="Hard";
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
                            score=(7-attempt)*level;
                            Console.WriteLine($"Congratulations! You guessed. {comment[attempt]} Your score is: {score}");
                            Console.ResetColor();

                            return score;
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
            return score;
        }
    }
}