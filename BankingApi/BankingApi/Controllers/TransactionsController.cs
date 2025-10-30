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
        // No try/catch needed!
        await _transactionService.DepositAsync(id, request.Amount);
        return Ok(new { message = "Deposit successful." });
    }

    [HttpPost("withdraw/{id}")]
    public async Task<IActionResult> Withdraw(Guid id, [FromBody] TransactionRequest request)
    {
        await _transactionService.WithdrawAsync(id, request.Amount);
        return Ok(new { message = "Withdrawal successful." });
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
    {
        await _transactionService.TransferAsync(request.FromAccountId, request.ToAccountId, request.Amount);
        return Ok(new { message = "Transfer successful." });
    }
}
