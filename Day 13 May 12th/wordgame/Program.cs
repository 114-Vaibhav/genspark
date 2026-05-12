namespace wordgame
{
    public class Program
    {
        public void start()
        {

            GameServices gameservice = new GameServices();
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("\n\n---------------Welcome to vaibhav's game world---------------");
            System.Console.WriteLine("---------------I have a surprise game for you---------------");
            Console.ForegroundColor = ConsoleColor.Cyan;
            System.Console.WriteLine("\n--------------------------------------------");
            System.Console.WriteLine("---------------GUESS THE WORD---------------");
            System.Console.WriteLine("--------------------------------------------\n");
            Console.ResetColor();
            gameservice.ShowRules();
            int choice=1;
            do
            {
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("--------------------------------------------");
                System.Console.WriteLine("Enter 1: Play Game");
                System.Console.WriteLine("Enter 2: SignUp as New User");
                System.Console.WriteLine("Enter 3: Login as User");
                System.Console.WriteLine("Enter 4: List your scores");
                System.Console.WriteLine("Enter 5: List Users with rank");
                System.Console.WriteLine("Enter 6: Exit");
                System.Console.WriteLine("--------------------------------------------\n");
                System.Console.WriteLine("Enter your choice: ");
                choice = Convert.ToInt32(Console.ReadLine());
                Console.ResetColor();

                switch (choice)
                {
                    case 1:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        gameservice.playGame();
                        System.Console.WriteLine("---------------------------------------\n");
                        break;
                    case 2:
                        Console.ForegroundColor = ConsoleColor.Green;
                        gameservice.SignUp();
                        System.Console.WriteLine("---------------------------------------\n");
                        break;
                    case 3:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        gameservice.Login();
                        System.Console.WriteLine("---------------------------------------\n");
                        break;
                    case 4:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        gameservice.ListScores();
                        System.Console.WriteLine("---------------------------------------\n");
                        break;
                    case 5:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        gameservice.ListUsersWithRank();
                        System.Console.WriteLine("--------------------------------------\n");
                        break;
                    case 6:
                        Console.WriteLine("Goodbye");
                        System.Console.WriteLine("---------------------------------------\n");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Exiting the game.");
                        break;
                }

            }while(choice!=6);
            
            
        }
    
        static void Main(string[] args)
        {
            new Program().start();
            // WordRepository wordRepository = new WordRepository();
            // wordRepository.getWordFromDatabase("Easy");
        }
    }
}