using BankingAPI.Interfaces;
using BankingAPI.Models;
using BankingAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using BankingAPI.Services;
// 000999887711
namespace BankingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ICustomerInteract _customerInteract;
        private readonly ITransaction _transaction ;
        public AccountController(ICustomerInteract customerInteract,
                                ITransaction transaction
        )
        {
            _customerInteract = customerInteract;
            _transaction = transaction;
        }
        [HttpPost]
        public ActionResult<CreateAccountResponse> CreateAccount(CreateAccountRequest account)
        {
            try
            {
                var result  = _customerInteract.OpensAccount(account);
                return Created("", result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpGet]
        public ActionResult<GetAccountResponse> GetAccount(string   accountNumber)
        {
            try
            {
                var account = _customerInteract.GetAccountByAccountNumber(accountNumber);
                if(account == null) 
                    return NotFound("No account with the given account number - "+accountNumber);
                return Ok(account);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
          
        [Authorize]
        [HttpPost("transfer")]
        public ActionResult<TransferResponse> Transfer(TransferRequest request)
        {
            try
            {
                var result = _transaction.Transfer(request);
                if(result.Status == "Failed")
                {
                    return BadRequest(result.message);
                } 
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("deposit")]
        public ActionResult<DepWithResponse> Deposit(DepWithRequest request)
        {
            try
            {
                var result = _transaction.Deposit(request);
                if(result.Status == "Failed")
                {
                    return BadRequest(result.message);
                } 
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("withdraw")]
        public ActionResult<DepWithResponse> Withdraw(DepWithRequest request)
        {
            try
            {
                var result = _transaction.Withdraw(request);
                if(result.Status == "Failed")
                {
                    return BadRequest(result.message);
                } 
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("statement")]
        public ActionResult<List<AccountStatement>> GetStatement(string accountNumber, int pageNo)
        {
            try
            {
                var result = _transaction.GetAccountStatement(new ACStatementRequest { AccountNumber = accountNumber, pageNo = pageNo });
                Console.WriteLine(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
       

    }
}
