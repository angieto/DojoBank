using Microsoft.EntityFrameworkCore;
namespace BankAccounts.Models
{
    public class BankContext: DbContext
    {
        // base(options) calls the parent class's constructor
        public BankContext(DbContextOptions<BankContext> options) : base(options) {}
        public DbSet<User> Users {get; set;}
        public DbSet<Transaction> Transactions {get; set;}
    }
}