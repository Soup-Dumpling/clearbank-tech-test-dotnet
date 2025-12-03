using System.Threading.Tasks;

namespace ClearBank.DeveloperTest.Core.UseCases.Account.GetAccount
{
    public interface IGetAccountRepository
    {
        Task<Models.Account> GetAccountByAccountNumberAsync(string accountNumber);
    }
}
