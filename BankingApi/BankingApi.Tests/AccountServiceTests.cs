using BankingApi.Application.Services;
using BankingApi.Core.Entities;
using BankingApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BankingApi.Tests
{
    public class AccountServiceTests : IDisposable
    {
        private readonly BankingDbContext _context;
        private readonly AccountService _service;

        // This is a setup method that runs BEFORE each test
        public AccountServiceTests()
        {
            var options = new DbContextOptionsBuilder<BankingDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new BankingDbContext(options);
            _context.Database.EnsureCreated();
            _service = new AccountService(_context);
        }

        // This is a cleanup method that runs AFTER each test
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // --- Test Setup Helper ---
        private async Task<Account> SeedAccount(string owner, decimal balance)
        {
            var account = new Account(owner, balance);
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            return account;
        }

        // --- CreateAccountAsync Tests ---

        [Fact]
        public async Task CreateAccountAsync_Success()
        {
            var ownerName = "Test User";
            var initialBalance = 100m;
            var newAccount = await _service.CreateAccountAsync(ownerName, initialBalance);
            Assert.NotNull(newAccount);
            var accountInDb = await _context.Accounts.FindAsync(newAccount.Id);
            Assert.NotNull(accountInDb);
            Assert.Equal(ownerName, accountInDb.OwnerName);
        }

        // --- GetAccountAsync Tests ---

        [Fact]
        public async Task GetAccountAsync_Success_ShouldReturnAccount()
        {
            var seededAccount = await SeedAccount("Get User", 150m);
            var result = await _service.GetAccountAsync(seededAccount.Id);
            Assert.NotNull(result);
            Assert.Equal(seededAccount.Id, result.Id);
        }

        [Fact]
        public async Task GetAccountAsync_Failure_ShouldReturnNull()
        {
            var nonExistentId = Guid.NewGuid();
            var result = await _service.GetAccountAsync(nonExistentId);
            Assert.Null(result);
        }

        // --- GetAllAccountsAsync Tests ---

        [Fact]
        public async Task GetAllAccountsAsync_Success_ShouldReturnAll()
        {
            await SeedAccount("User 1", 10m);
            await SeedAccount("User 2", 20m);
            var result = await _service.GetAllAccountsAsync();
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllAccountsAsync_Success_ShouldReturnEmptyList()
        {
            var result = await _service.GetAllAccountsAsync();
            Assert.Empty(result);
        }

        // --- DepositAsync Tests ---

        [Fact]
        public async Task DepositAsync_Success()
        {
            var account = await SeedAccount("Deposit User", 100m);
            await _service.DepositAsync(account.Id, 50m);
            var updatedAccount = await _context.Accounts.FindAsync(account.Id);
            Assert.Equal(150m, updatedAccount.Balance);
        }

        [Fact]
        public async Task DepositAsync_Failure_NegativeAmount()
        {
            var account = await SeedAccount("Deposit User", 100m);
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.DepositAsync(account.Id, -50m)
            );
            var updatedAccount = await _context.Accounts.FindAsync(account.Id);
            Assert.Equal(100m, updatedAccount.Balance); // Balance unchanged
        }

        [Fact]
        public async Task DepositAsync_Failure_AccountNotFound()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.DepositAsync(Guid.NewGuid(), 50m)
            );
        }

        // --- WithdrawAsync Tests ---

        [Fact]
        public async Task WithdrawAsync_Success()
        {
            var account = await SeedAccount("Withdraw User", 100m);
            await _service.WithdrawAsync(account.Id, 30m);
            var updatedAccount = await _context.Accounts.FindAsync(account.Id);
            Assert.Equal(70m, updatedAccount.Balance);
        }

        [Fact]
        public async Task WithdrawAsync_Failure_InsufficientFunds()
        {
            var account = await SeedAccount("Withdraw User", 100m);
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.WithdrawAsync(account.Id, 150m)
            );
            var updatedAccount = await _context.Accounts.FindAsync(account.Id);
            Assert.Equal(100m, updatedAccount.Balance); // Balance unchanged
        }

        [Fact]
        public async Task WithdrawAsync_Failure_NegativeAmount()
        {
            var account = await SeedAccount("Withdraw User", 100m);
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.WithdrawAsync(account.Id, -50m)
            );
        }

        [Fact]
        public async Task WithdrawAsync_Failure_AccountNotFound()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.WithdrawAsync(Guid.NewGuid(), 50m)
            );
        }

        // --- TransferAsync Tests (With both fixes) ---

        [Fact]
        public async Task TransferAsync_Success()
        {
            // Arrange
            var fromAccount = await SeedAccount("From User", 1000m);
            var toAccount = await SeedAccount("To User", 500m);

            // Act
            await _service.TransferAsync(fromAccount.Id, toAccount.Id, 200m);

            // Assert (Re-fetch to check saved values)
            var updatedFrom = await _context.Accounts.FindAsync(fromAccount.Id);
            var updatedTo = await _context.Accounts.FindAsync(toAccount.Id);
            Assert.Equal(800m, updatedFrom.Balance);
            Assert.Equal(700m, updatedTo.Balance);
        }

        [Fact]
        public async Task TransferAsync_Failure_InsufficientFunds()
        {
            // Arrange
            var fromAccount = await SeedAccount("From User", 100m);
            var toAccount = await SeedAccount("To User", 500m);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.TransferAsync(fromAccount.Id, toAccount.Id, 200m)
            );

            Assert.Equal("Insufficient funds for transfer.", ex.Message);

            // Verify rollback (Re-fetch to check saved values)
            var updatedFrom = await _context.Accounts.FindAsync(fromAccount.Id);
            var updatedTo = await _context.Accounts.FindAsync(toAccount.Id);
            Assert.Equal(100m, updatedFrom.Balance);
            Assert.Equal(500m, updatedTo.Balance);
        }

        [Fact]
        public async Task TransferAsync_Failure_NegativeAmount()
        {
            // Arrange
            var fromAccount = await SeedAccount("From User", 1000m);
            var toAccount = await SeedAccount("To User", 500m);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.TransferAsync(fromAccount.Id, toAccount.Id, -200m)
            );

            // Verify balances unchanged (Re-fetch)
            var updatedFrom = await _context.Accounts.FindAsync(fromAccount.Id);
            var updatedTo = await _context.Accounts.FindAsync(toAccount.Id);
            Assert.Equal(1000m, updatedFrom.Balance);
            Assert.Equal(500m, updatedTo.Balance);
        }

        [Fact]
        public async Task TransferAsync_Failure_FromAccountNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var toAccount = await SeedAccount("To User", 500m);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.TransferAsync(nonExistentId, toAccount.Id, 200m)
            );

            // Verify balance unchanged (Re-fetch)
            var updatedTo = await _context.Accounts.FindAsync(toAccount.Id);
            Assert.Equal(500m, updatedTo.Balance);
        }
    }
}
