using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankingAPI.Interfaces;
using BankingAPI.Models;
using BankingAPI.Misc;
using BankingAPI.Models.DTOs;
using System.Security.Cryptography;
using System.Security.Authentication;
using BankingAPI.Migrations;
using BankingAPI.Repositories;



namespace BankingAPI.Services
{
    public class TransactionService : ITransaction
    {
        readonly IRepository<string, Account> _accountRespository;
        private readonly IRepository<int, Transaction> _transactionRepository;
        private readonly IRepository<int,AccountStatement> _accountStatementRepository;
        private readonly StatementRepository _statementRepository;
        // private readonly StatementRepository statementRepository= new StatementRepository();

        public TransactionService(IRepository<string,Account> accountRepository,
                                IRepository<int, Transaction> transactionRepository,
                                IRepository<int,AccountStatement> accountStatementRepository,
                                StatementRepository statementRepository
                             
                                )
        {
            _accountRespository = accountRepository;
            _transactionRepository = transactionRepository;
            _accountStatementRepository=accountStatementRepository;
            _statementRepository = statementRepository;
            

        }


        public TransferResponse Transfer(TransferRequest request)
        {
            var sourceAccount = _accountRespository.Get(request.fromAccountNo);
            var destinationAccount = _accountRespository.Get(request.toAccountNo);
            if (sourceAccount == null || destinationAccount == null)
            {
                return new TransferResponse
                {
                    sentAmount = request.Amount,
                    Status = "Failed",
                    transactionDate = DateTime.UtcNow,
                    message = "Source or destination account not found"
                };
                throw new UnableToCreateEntityException("Source or destination account not found");
                
            }
            if (sourceAccount.Balance < request.Amount)
            {
                return new TransferResponse
                {
                    fromAccountNo = sourceAccount.AccountNumber,
                    toAccountNo = destinationAccount.AccountNumber,
                    sentAmount = request.Amount,
                    Status = "Failed",
                    transactionDate = DateTime.UtcNow,
                    message = "Insufficient balance"
                };
                throw new UnableToCreateEntityException("Insufficient balance");
                
            }
            sourceAccount.Balance -= request.Amount;
            destinationAccount.Balance += request.Amount;
            _accountRespository.Update(sourceAccount.AccountNumber, sourceAccount);
            _accountRespository.Update(destinationAccount.AccountNumber, destinationAccount);
            _transactionRepository.Create(new Transaction
            {
                TransactionDate = DateTime.UtcNow,
                FromAccountNumber = sourceAccount.AccountNumber,
                ToAccountNumber = destinationAccount.AccountNumber,
                Status = "Success",
                
            }); 
            _accountStatementRepository.Create(new AccountStatement
            {
               TransactionDate = DateTime.UtcNow,
               AccountNumber = sourceAccount.AccountNumber,
               Debit = request.Amount,
               Credit = 0,
               Balance = sourceAccount.Balance 
            });
            _accountStatementRepository.Create(new AccountStatement
            {
               TransactionDate = DateTime.UtcNow,
               AccountNumber = destinationAccount.AccountNumber,
               Debit = 0,
               Credit = request.Amount,
               Balance = sourceAccount.Balance 
            });
            
            return new TransferResponse
            {
                fromAccountNo = sourceAccount.AccountNumber,
                toAccountNo = destinationAccount.AccountNumber,
                sentAmount = request.Amount,
                RemainedBalance = sourceAccount.Balance,
                TransactionReferenceNumber= Guid.NewGuid().ToString(),
                Status = "Success",
                transactionDate = DateTime.UtcNow,
                message = "Transfer successful"

            };
        }


        public DepWithResponse Deposit(DepWithRequest request)
        {
            var account = _accountRespository.Get(request.AccountNumber);
            if (account == null)
            {
                return new DepWithResponse
                {
                    AccountNumber = request.AccountNumber,
                    Amount = request.Amount,
                    Status = "Failed",
                    transactionDate = DateTime.UtcNow,
                    transactionType = TransactionType.Deposit,
                    message = "Account not found"
                };
                throw new UnableToCreateEntityException("Account not found");
                
            }
            account.Balance += request.Amount;
            _accountRespository.Update(account.AccountNumber, account);
            _accountStatementRepository.Create(new AccountStatement
            {
               TransactionDate = DateTime.UtcNow,
               AccountNumber = account.AccountNumber,
               Debit = 0,
               Credit = request.Amount,
               Balance = account.Balance 
            });
            return new DepWithResponse
            {
                AccountNumber = account.AccountNumber,
                Amount = request.Amount,
                Status = "Success",
                transactionDate = DateTime.UtcNow,
                transactionType = TransactionType.Deposit,
                message = "Deposit successful"
            };

        }

        
        public DepWithResponse Withdraw(DepWithRequest request)
        {
            var account = _accountRespository.Get(request.AccountNumber);
            if (account == null)
            {
                return new DepWithResponse
                {
                    AccountNumber = request.AccountNumber,
                    Amount = request.Amount,
                    Status = "Failed",
                    transactionDate = DateTime.UtcNow,
                    transactionType = TransactionType.Withdraw,
                    message = "Account not found"
                };
                throw new UnableToCreateEntityException("Account not found");
                
            }
            if (account.Balance < request.Amount)
            {
                return new DepWithResponse
                {
                    AccountNumber = account.AccountNumber,
                    Amount = request.Amount,
                    Status = "Failed",
                    transactionDate = DateTime.UtcNow,
                    transactionType = TransactionType.Withdraw,
                    message = "Insufficient balance"
                };
                throw new UnableToCreateEntityException("Insufficient balance");
                
            }
            account.Balance -= request.Amount;
            _accountRespository.Update(account.AccountNumber, account);
            _accountStatementRepository.Create(new AccountStatement
            {
               TransactionDate = DateTime.UtcNow,
               AccountNumber = account.AccountNumber,
               Debit = request.Amount,
               Credit = 0,
               Balance = account.Balance 
            });
            return new DepWithResponse
            {
                AccountNumber = account.AccountNumber,
                Amount = request.Amount,
                Status = "Success",
                transactionDate = DateTime.UtcNow,
                transactionType = TransactionType.Deposit,
                message = "Withdraw successful"
            };
        }

        public List<AccountStatement> GetAccountStatement(ACStatementRequest request)
        {
            var account = _accountRespository.Get(request.AccountNumber);
            if (account == null)
            {
                throw new UnableToCreateEntityException("Account not found");
                
            }
            var accountStatements = _statementRepository.GetAccountStatements(request.AccountNumber, request.pageNo);
            return accountStatements;   
        }

    }
}
