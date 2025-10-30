namespace BankingApi.Application.DTOs;

public class AccountResponse
{
    public Guid Id { get; set; }
    public string OwnerName { get; set; }
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
}
