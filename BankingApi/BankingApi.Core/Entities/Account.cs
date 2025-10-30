namespace BankingApi.Core.Entities;

public class Account
{
    public Guid Id { get; set; }
    public string OwnerName { get; set; }
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
    private Account() { }

    public Account(string ownerName, decimal initialBalance)
    {
        Id = Guid.NewGuid();
        OwnerName = ownerName;
        Balance = initialBalance;
        CreatedAt = DateTime.UtcNow;
    }
}
