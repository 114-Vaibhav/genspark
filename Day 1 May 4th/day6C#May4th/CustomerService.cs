namespace UnderstandingOOPs
{
//     Enhance the application to interact with the customer on loop mode.
 
// 1. Add account
// 2. Print Account details giving account number
// 3. Print account details using phone number
// 4. Exit
    internal class CustomerService : ICustomerInteract
    {
        
        List<Account> accounts = new List<Account>();
        static string lastAccountNumber = "9990001000";
        public Account OpensAccount()
        {
            Account account = TakeCustomerDetails();
            // TakeCustomerDetails();
            TakeInitialDeposit(account);
            long accNum = Convert.ToInt64(lastAccountNumber);
            account.AccountNo=  (++accNum).ToString();
            lastAccountNumber = accNum.ToString();
            accounts.Add(account);
            return account;
        }   
        private void TakeInitialDeposit(Account account)
        {
            System.Console.WriteLine("Enter initial amount: ");
            float amount=0;
            while(!float.TryParse(Console.ReadLine(),out amount))
                System.Console.WriteLine("Invalid try again");
            account.Balance+=amount;
        }
         private Account TakeCustomerDetails(){
            Account account;
            System.Console.WriteLine("select account type 1 for saving 2 for current");
            int typeChoice;
            while(!Int32.TryParse(Console.ReadLine(), out typeChoice) && typeChoice>0 && typeChoice<3)
                System.Console.WriteLine("Try again");
            if(typeChoice==1) account = new SavingAccount();
            else account = new CurrAccount();
            System.Console.WriteLine("Enter full name");
            account.NameOnAccount = Console.ReadLine()??"";
            System.Console.WriteLine("Enter dob in yyyy-mm-dd format");
            DateTime dob;
            
            while(!DateTime.TryParse(Console.ReadLine(),out dob) && dob>DateTime.Today)
                System.Console.WriteLine("Invalid try again");
            account.DateOfBirth=dob;

            System.Console.WriteLine("enter email: ");
            account.Email = Console.ReadLine()??"";
            System.Console.WriteLine("enter Phone: ");
            account.Phone = Console.ReadLine()??"";
            return account;
    } 
    
        public void PrintAccountDetails(string accountno)
        {
            
            Account account=null;
            foreach(var item in accounts)
            {
                if (item.AccountNo == accountno)
                {
                    account =item;
                    break;
                }
            }
            if (account != null)
            {
            PrintAccount(account);
            return;                
            }
            System.Console.WriteLine("No account found");
        }
        public void PrintAccountDetailsUsingPhone(string phoneno)
        {
            Account account=null;
            foreach(var item in accounts)
            {
                if (item.Phone == phoneno)
                {
                    account =item;
                    break;
                }
            }
            if (account != null)
            {
            PrintAccount(account);
            return;                
            }
            System.Console.WriteLine("No account found");
        }
        private void PrintAccount(Account account)
        {
            System.Console.WriteLine("--------Account Details------");
            Console.WriteLine(account);
            Console.WriteLine("-----------------------------");
        }
    }
   
    
}