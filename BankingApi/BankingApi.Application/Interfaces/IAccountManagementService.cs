using BankingApi.Core.Entities;

namespace BankingApi.Application.Interfaces;

public interface IAccountManagementService
{
    Task<Account> CreateAccountAsync(string ownerName, decimal initialBalance);
    Task<Account?> GetAccountAsync(Guid id);
    Task<IEnumerable<Account>> GetAllAccountsAsync();
}
