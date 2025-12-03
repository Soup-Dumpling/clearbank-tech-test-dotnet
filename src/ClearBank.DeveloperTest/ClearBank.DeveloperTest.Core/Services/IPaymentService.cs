using ClearBank.DeveloperTest.Core.UseCases.Payment.MakePayment;
using System.Threading.Tasks;

namespace ClearBank.DeveloperTest.Core.Services
{
    public interface IPaymentService
    {
        Task<MakePaymentResult> MakePayment(MakePaymentRequest request);
    }
}
