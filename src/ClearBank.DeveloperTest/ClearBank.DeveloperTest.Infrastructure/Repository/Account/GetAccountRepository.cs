using ClearBank.DeveloperTest.Core.UseCases.Account.GetAccount;
using System.Threading.Tasks;

namespace ClearBank.DeveloperTest.Infrastructure.Repository.Account
{
    public class GetAccountRepository : IGetAccountRepository
    {
        private readonly ClearBankContext context;

        public GetAccountRepository(ClearBankContext context) 
        {
            this.context = context;
        }

        public async Task<Core.Models.Account> GetAccountByAccountNumberAsync(string accountNumber)
        {
            var account = await context.Accounts.FindAsync(accountNumber);
            return account;
        }
    }
}
