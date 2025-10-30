using BankingApi.Core.Entities;
using Microsoft.EntityFrameworkCore;
namespace BankingApi.Infrastructure.Data;

public class BankingDbContext : DbContext
{
    public BankingDbContext(DbContextOptions<BankingDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
}
