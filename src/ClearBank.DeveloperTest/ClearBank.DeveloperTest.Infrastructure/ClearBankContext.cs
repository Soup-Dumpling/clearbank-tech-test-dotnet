using ClearBank.DeveloperTest.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;

namespace ClearBank.DeveloperTest.Infrastructure
{
    public class ClearBankContext : DbContext
    {
        public ClearBankContext(DbContextOptions<ClearBankContext> contextOptions) : base(contextOptions) { }
        public DbSet<Core.Models.Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AccountMapping());
        }
    }
}
