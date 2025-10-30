namespace BankingApi.Application.Interfaces;

public interface ITransactionService
{
    Task DepositAsync(Guid id, decimal amount);
    Task WithdrawAsync(Guid id, decimal amount);
    Task TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount);
}
