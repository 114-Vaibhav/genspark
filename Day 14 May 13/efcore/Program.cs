using UnderstandingEfCoreApp.Contexts;

namespace UnderstandingEfCoreApp
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            using BankingContext context = new BankingContext();

            Console.WriteLine("Database connection successful.");
        }
    }
}