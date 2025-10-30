using BankingApi.Application.DTOs;
using BankingApi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost("deposit/{id}")]
    public async Task<IActionResult> Deposit(Guid id, [FromBody] TransactionRequest request)
    {
        try
        {
            await _transactionService.DepositAsync(id, request.Amount);
            return Ok(new { message = "Deposit successful." });
        }
        catch (Exception ex)
        {]
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("withdraw/{id}")]
    public async Task<IActionResult> Withdraw(Guid id, [FromBody] TransactionRequest request)
    {
        try
        {
            await _transactionService.WithdrawAsync(id, request.Amount);
            return Ok(new { message = "Withdrawal successful." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
    {
        try
        {
            await _transactionService.TransferAsync(request.FromAccountId, request.ToAccountId, request.Amount);
            return Ok(new { message = "Transfer successful." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
