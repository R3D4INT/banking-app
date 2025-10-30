using AutoMapper;
using BankingApi.Application.DTOs;
using BankingApi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountManagementService _accountService;
    private readonly IMapper _mapper; 
    public AccountsController(IAccountManagementService accountService, IMapper mapper)
    {
        _accountService = accountService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        var account = await _accountService.CreateAccountAsync(request.OwnerName, request.InitialBalance);
        var response = _mapper.Map<AccountResponse>(account);

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

        var response = _mapper.Map<AccountResponse>(account);

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAccounts()
    {
        var accounts = await _accountService.GetAllAccountsAsync();
        var response = _mapper.Map<IEnumerable<AccountResponse>>(accounts);

        return Ok(response);
    }
}
