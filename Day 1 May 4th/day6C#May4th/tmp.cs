// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;

// namespace UnderstandingOOPSApp.Models
// {
//     public enum AccType
//     {
//         SavingAccount =1,CurrentAccount=2
//     }
//     internal class Account 
//     {
        

//         public  string AccountNumber { get; set; } =string.Empty;
//         public string NameOnAccount { get; set; } = string.Empty;
//         public DateTime DateOfBirth { get; set; }
//         public string Email { get; set; } = string.Empty;
//         public string Phone { get; set; } = string.Empty;
//         public float Balance { get; set; }
//         public AccType AccountType { get; set; }
//         public Account()
//         {
            
//         }

//         public Account(string accountNumber, string nameOnAccount, DateTime dateOfBirth, string email, string phone, float balance)
//         {
//             AccountNumber = accountNumber;
//             NameOnAccount = nameOnAccount;
//             DateOfBirth = dateOfBirth;
//             Email = email;
//             Phone = phone;
//             Balance = balance;
//         }
//         public override string ToString()
//         {
//             return $"Account Number : {AccountNumber}\nAccountType : {AccountType}\nAccount Holder Name : {NameOnAccount}\nPhone Number : {Phone}\n" +
//                 $"Email : {Email}\nBalance : ${Balance}";
//         }
//     }
// }

// -------------------------------------------------------------------------------------------

// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;

// namespace UnderstandingOOPSApp.Models
// {
//     internal class CurrentAccount : Account
//     {
//         public CurrentAccount()
//         {
//             AccountType = AccType.CurrentAccount;
//             Balance = 0.0f;
//         }
       
//     }
// }



// -------------------------------------------------------------------------------------------

// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;

// namespace UnderstandingOOPSApp.Models
// {
//     internal class SavingAccount :Account
//     {
//         public SavingAccount()
//         {
//             AccountType = AccType.SavingAccount;
//             Balance = 100.0f;
//         }
//     }
// }



// -------------------------------------------------------------------------------------------

// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using UnderstandingOOPSApp.Models;

// namespace UnderstandingOOPSApp.Interfaces
// {
//     internal interface ICustomerInteract
//     {
//         public Account OpensAccount();
//         public void PrintAccountDetails(string accountNumber);

//     }
// }



// -------------------------------------------------------------------------------------------


// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using UnderstandingOOPSApp.Interfaces;
// using UnderstandingOOPSApp.Models;

// namespace UnderstandingOOPSApp.Services
// {
//     internal class CustomerService : ICustomerInteract
//     {
//         List<Account> accounts = new List<Account>();
//         static string lastAccountNumber = "9990001000";
//         public Account OpensAccount()
//         {
//             Account account = TakeCustomerDetails();
//             TakeInitialDeposit(account);
//             long accNum = Convert.ToInt64(lastAccountNumber);
//             account.AccountNumber =  (++accNum).ToString();
//             lastAccountNumber = accNum.ToString();
//             accounts.Add(account);
//             return account;
//         }

//         private void TakeInitialDeposit(Account account)
//         {
//             Console.WriteLine("Do you want to deposit any amount now. If yes enter the amount. else enter 0.?");
//             float amount = 0;
//             while(!float.TryParse(Console.ReadLine(), out amount))
//                 Console.WriteLine("Invalid entry. Please enter the deposit amount");
//             account.Balance += amount;

//         }

//         private Account TakeCustomerDetails()
//         {
//             Account account = new Account();
//             Console.WriteLine("Please select the type of account you want to open. 1 for savings. 2 for current");
//             int typeChoice;
//             while(!int.TryParse(Console.ReadLine(), out typeChoice) && typeChoice>0 && typeChoice<3)
//                 Console.WriteLine("Invalid entry. Please try again");
//             if(typeChoice == 1)
//                 account = new SavingAccount();
//             if(typeChoice == 2)
//                 account = new CurrentAccount();
//             Console.WriteLine("Please enter your full name");
//             account.NameOnAccount = Console.ReadLine()??"";
//             Console.WriteLine("Please enter your Date of birth in yyyy-mm--dd format");
//             DateTime dob;
//             while(!DateTime.TryParse(Console.ReadLine(),out dob ) && dob>DateTime.Today)
//                 Console.WriteLine("Invalid entry for date. Please try again");
//             Console.WriteLine("Please enter your email address");
//             account.Email = Console.ReadLine() ?? "";
//             Console.WriteLine("Please enter your phone number");
//             account.Phone = Console.ReadLine() ?? "";
//             return account;

//         }

//         public void PrintAccountDetails(string accountNumber)
//         {
//             Account account = null;
//             foreach (var item in accounts)
//             {
//                 if(item.AccountNumber == accountNumber)
//                 {
//                     account = item;
//                     break;
//                 }
//             }
//             if (account != null)
//             {
//                 PrintAccount(account);
//                 return;
//             }
//             Console.WriteLine("No account with the given number is present with us");
            
//         }

//         private void PrintAccount(Account account)
//         {
//             Console.WriteLine("-----------------------------");
//             Console.WriteLine(account);
//             Console.WriteLine("-----------------------------");
//         }
//     }
// }


// -------------------------------------------------------------------------------------------

// using UnderstandingOOPSApp.Interfaces;
// using UnderstandingOOPSApp.Services;

// namespace UnderstandingOOPSApp
// {
//     internal class Program
//     {
//         ICustomerInteract customerInteract;
//         public Program()
//         {
//             customerInteract = new CustomerService();
//         }
//         void DoBanking()
//         {
//             var account = customerInteract.OpensAccount();
//             Console.WriteLine(account);
//             Console.WriteLine("Please enter the account you like see");
//             string accNum = Console.ReadLine()??"";
//             customerInteract.PrintAccountDetails(accNum);
            
//         }
//         static void Main(string[] args)
//         {
//             new Program().DoBanking();
//         }
//     }
// }




// -------------------------------------------------------------------------------------------





// -------------------------------------------------------------------------------------------




// -------------------------------------------------------------------------------------------