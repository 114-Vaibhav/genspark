
using Microsoft.EntityFrameworkCore;
using dummydbtest.Models;


namespace dummydbtest
{
    internal class Program
    {
        readonly TempdvdContext _context;
        Program()
        {
            _context = new TempdvdContext();
        }
        void TransactWithTransationinDatabase()
        {
            using var transaction = _context.Database.BeginTransaction();
                Account fc = new Account() { Accno = 4};
                Account tc = new Account() { Accno = 2 };
                float amount =5114;
                fc = _context.Accounts.FirstOrDefault(a => a.Accno == fc.Accno);
                tc = _context.Accounts.FirstOrDefault(a => a.Accno == tc.Accno);
                int tran_id = 7;
                if (fc == null || tc == null)
            {
                Console.WriteLine("Account not found");
                return;
            }
                
            try
            {
                _context.Database.ExecuteSqlInterpolated($"call add_trans({tran_id},{fc.Accno},{tc.Accno},{amount});");
                if(fc.Balance-amount<=0)
                {
                    throw new Exception("Insufficient Balance");
                    // Console.WriteLine("Insufficient Balance");
                    // return;
                }
                _context.Database.ExecuteSqlInterpolated($"call update_account({fc.Accno},{fc.Balance-amount});");
                _context.Database.ExecuteSqlInterpolated($"call update_account({tc.Accno},{tc.Balance+amount});");
               transaction.Commit();
                Console.WriteLine("Transaction Completed");
                // _context.Accounts.Add(account1);
                // _context.Accounts.Add(account2);
                // _context.SaveChanges();
                // transaction.Commit();
                // Console.WriteLine("Accounts Created");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine(ex.Message);
            }
        }
        void AddAccountUsingSP()
        {
            Account account = new Account() { Accno = 5, Balance = 1233.3f };
            //call add_account(4,3243);
            _context.Database.ExecuteSqlInterpolated($"call add_account({account.Accno},{account.Balance});");
            Console.WriteLine("Account Created");
        }
        static void Main(string[] args)
        {
            // new Program().AddAccountUsingSP();
                new Program().TransactWithTransationinDatabase();
        }
    }
}




