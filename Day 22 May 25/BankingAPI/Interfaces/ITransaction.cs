using BankingAPI.Models;
using BankingAPI.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BankingAPI.Interfaces
{
    public interface ITransaction
    {
        public TransferResponse Transfer(TransferRequest request);
        public DepWithResponse Deposit(DepWithRequest request);
        public DepWithResponse Withdraw(DepWithRequest request);
        public List<AccountStatement> GetAccountStatement(ACStatementRequest request);
    
    }
}
