namespace BankingApi.Application.DTOs;

public class CreateAccountRequest
{
    public string OwnerName { get; set; }
    public decimal InitialBalance { get; set; }
}
