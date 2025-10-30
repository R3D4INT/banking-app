using BankingApi.Application.Interfaces;
using BankingApi.Core.Entities;
using BankingApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Application.Services;

public class AccountService : IAccountManagementService, ITransactionService
{
    private readonly BankingDbContext _context;

    public AccountService(BankingDbContext context)
    {
        _context = context;
    }

    // --- IAccountManagementService Implementation ---

    public async Task<Account> CreateAccountAsync(string ownerName, decimal initialBalance)
    {
        var account = new Account(ownerName, initialBalance);
        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task<Account?> GetAccountAsync(Guid id)
    {
        return await _context.Accounts.FindAsync(id);
    }

    public async Task<IEnumerable<Account>> GetAllAccountsAsync()
    {
        return await _context.Accounts.ToListAsync();
    }

    // --- ITransactionService Implementation ---

    public async Task DepositAsync(Guid id, decimal amount)
    {
        var account = await GetAccountOrThrowAsync(id);
        if (amount <= 0)
        {
            throw new ArgumentException("Deposit amount must be positive.", nameof(amount));
        }

        account.Balance += amount;
        await _context.SaveChangesAsync();
    }

    public async Task WithdrawAsync(Guid id, decimal amount)
    {
        var account = await GetAccountOrThrowAsync(id);

        if (amount <= 0)
        {
            throw new ArgumentException("Withdrawal amount must be positive.", nameof(amount));
        }
        if (account.Balance < amount)
        {
            throw new InvalidOperationException("Insufficient funds.");
        }

        account.Balance -= amount;
        await _context.SaveChangesAsync();
    }

    public async Task TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Transfer amount must be positive.", nameof(amount));
        }

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                var fromAccount = await GetAccountOrThrowAsync(fromAccountId);
                var toAccount = await GetAccountOrThrowAsync(toAccountId);

                if (fromAccount.Balance < amount)
                {
                    throw new InvalidOperationException("Insufficient funds for transfer.");
                }

                fromAccount.Balance -= amount;
                toAccount.Balance += amount;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

    private async Task<Account> GetAccountOrThrowAsync(Guid id)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (account == null)
        {
            throw new KeyNotFoundException($"Account with id {id} not found.");
        }
        return account;
    }
}
