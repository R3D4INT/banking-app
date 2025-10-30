using BankingApi.Application.DTOs;
using BankingApi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountManagementService _accountService;

    public AccountsController(IAccountManagementService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        var account = await _accountService.CreateAccountAsync(request.OwnerName, request.InitialBalance);

        var response = new AccountResponse
        {
            Id = account.Id,
            OwnerName = account.OwnerName,
            Balance = account.Balance,
            CreatedAt = account.CreatedAt
        };

        return CreatedAtAction(nameof(GetAccount), new { id = account.Id }, response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAccount(Guid id)
    {
        var account = await _accountService.GetAccountAsync(id);
        if (account == null)
        {
            return NotFound();
        }

        var response = new AccountResponse
        {
            Id = account.Id,
            OwnerName = account.OwnerName,
            Balance = account.Balance,
            CreatedAt = account.CreatedAt
        };

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAccounts()
    {
        var accounts = await _accountService.GetAllAccountsAsync();

        var response = accounts.Select(account => new AccountResponse
        {
            Id = account.Id,
            OwnerName = account.OwnerName,
            Balance = account.Balance,
            CreatedAt = account.CreatedAt
        });

        return Ok(response);
    }
}
