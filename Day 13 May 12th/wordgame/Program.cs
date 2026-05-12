namespace wordgame
{
    public class Program
    {
        static void Main()
        {
            Game game = new Game();
            Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("\n\n---------------Welcome to vaibhav's game world---------------");
            System.Console.WriteLine("---------------I have a surprise game for you---------------");
            Console.ForegroundColor = ConsoleColor.Cyan;
            System.Console.WriteLine("\n--------------------------------------------");
            System.Console.WriteLine("---------------GUESS THE WORD---------------");
            System.Console.WriteLine("--------------------------------------------\n");
            Console.ResetColor();
            game.ShowRules();
            int choice=1;
            do
            {
                game.playGame();
                
                System.Console.WriteLine("Enter 1 to Play Again, Enter 0 to Exit");
                choice = Convert.ToInt32(Console.ReadLine());
            }while(choice==1);
            
            
        }
    }
}