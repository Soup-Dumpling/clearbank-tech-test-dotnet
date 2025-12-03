using System.Threading.Tasks;

namespace ClearBank.DeveloperTest.Core.UseCases.Payment.MakePayment
{
    public interface IMakePaymentRepository
    {
        Task UpdateAccounts();
    }
}
