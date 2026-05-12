using UnderstandingEfCoreApp.Contexts;

namespace UnderstandingEfCoreApp
{
    internal class Program
    {
        void InsertCustomer()
        {
            Customer customer = new Customer()
            {
                Name = "John Doe",
                Phone = "123-456-7890",
                Email = ""
            };
        }
        static void Main(string[] args)
        {
            using BankingContext context = new BankingContext();

            Console.WriteLine("Database connection successful.");
        }
    }
}