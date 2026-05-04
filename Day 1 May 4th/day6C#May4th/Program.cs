using UnderstandingOOPs;

namespace UnderstandingOOPs
{

    internal class Program{
    ICustomerInteract customerInteract;
        public Program(){
            customerInteract = new CustomerService();
        }
        void DoBanking()
        {
//             Enhance the application to interact with the customer on loop mode.
 
// 1. Add account
// 2. Print Account details giving account number
// 3. Print account details using phone number
// 4. Exit
            int choice =0;
            while (choice != 4)
            {   
                System.Console.WriteLine("1. Add account ");
                System.Console.WriteLine("2. Print Account details giving account number ");
                System.Console.WriteLine("3. Print account details using phone number ");
                System.Console.WriteLine("4. Exit");
                System.Console.WriteLine("Enter your choice: ");
                choice = Convert.ToInt32(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        var account = customerInteract.OpensAccount();
                        System.Console.WriteLine(account);
                        break;

                    case 2:
                        System.Console.WriteLine("Please enter account");
                        string accno = Console.ReadLine();
                        customerInteract.PrintAccountDetails(accno);
                        break;

                    case 3:
                        System.Console.WriteLine("Please enter phone number");
                        string phoneno = Console.ReadLine();
                        customerInteract.PrintAccountDetailsUsingPhone(phoneno);
                        break;

                    default:
                        System.Console.WriteLine("Invalid choice");
                        break;
                }
                // if (choice == 1)
                // {
                //     var account= customerInteract.OpensAccount();
                //     System.Console.WriteLine(account);
                // }else if (choice == 2)
                // {
                //     System.Console.WriteLine("Please enter account");
                //     string accno = Console.ReadLine();
                //     customerInteract.PrintAccountDetails(accno);
                // }else if (choice == 3)
                // {
                //     System.Console.WriteLine("Please enter phone number");
                //     string phoneno = Console.ReadLine();
                //     // customerInteract.PrintAccountDetails(phoneno,3);
                //     customerInteract.PrintAccountDetailsUsingPhone(phoneno);
                // }
            }
           
           
        }
        static void Main(string[] args)
        {
            new Program().DoBanking();
            // Account account = new Account("0000114",
            //     "vaibhav",
            //     new DateTime(2000,12,12),
            //     "ab@ab",
            //     "1234567890",
            //     100.00f
            // );
            // System.Console.WriteLine(account);
        }
    }
    
}