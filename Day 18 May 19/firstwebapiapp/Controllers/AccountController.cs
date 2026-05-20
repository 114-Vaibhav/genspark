using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using firstwebapiapp;
namespace firstwebapiapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        static List<Account> accounts = new List<Account>
        {
            new Account{AccountNumber="0009998787",Balance= 100000,OpeninigDate=new DateTime(2026,1,14),Status="Active"},
            new Account{AccountNumber="0009998789",Balance= 100030,OpeninigDate=new DateTime(2026,2,14),Status = "Active"}
        };
        static List<Transaction> transactions = new List<Transaction>{};
        [HttpGet]
        public ActionResult<IEnumerable<Account>> Get()
        {
            if(accounts.Count == 0)
                return NotFound("No Accounts in the bank yet");
            return Ok(accounts);
        }

        [HttpGet("GetAccountByNumebr")]
        public ActionResult<Account> Get(string accountNumber)
        {
            if (accounts.Count == 0)
                return NotFound("No Accounts in the bank yet");
            var account = accounts.SingleOrDefault(a=>a.AccountNumber == accountNumber);
            if (account == null)
                return NotFound("No accont with the given account number");
            return Ok(account);
        }
        [HttpPost]
        public ActionResult<Account> Post([FromBody] Account account)
        {
            accounts.Add(account);
            return Created("https://localhost:7280/api/Account/GetAccountByNumebr?accountNumber="+account.AccountNumber, account);
        }
        [HttpPost("Transfer")]
        public ActionResult<Transaction> Post([FromBody] Transaction transaction)
        {
            var fromAccount = accounts.Single(a=>a.AccountNumber == transaction.FromAccountNumber);
            var toAccount = accounts.Single(a=>a.AccountNumber == transaction.ToAccountNumber);
            if(fromAccount == null || toAccount == null)
            {
                return NotFound("No accont with the given account number");
            }
            if(fromAccount.Balance < transaction.Amount)
            {
                return NotFound("Insufficient Balance");
            }
            fromAccount.Balance -= transaction.Amount;
            toAccount.Balance += transaction.Amount;
            accounts.Remove(fromAccount);
            accounts.Remove(toAccount);
            accounts.Add(fromAccount);
            accounts.Add(toAccount);
            transactions.Add(transaction);
            return Created("Transaction Successful", transaction);
        }


    }
}
    // [Route("api/[controller]")]
    // [ApiController]
    // public class AccountController : ControllerBase
    // {
    //     [HttpGet]
    //     public string Greet()
    //     {
    //         return "Hello World!!";
    //     }

    //       [HttpGet("GreetWithName")]
    //       public string Greet(string name)
    //       {
    //           return $"Hello {name}!!";
    //       }
    //       [HttpPost]
    //       public string GreetPost(Account account)
    //       {
    //           return $"Hello {account.Name}!!";
    //       }
    // }
   



