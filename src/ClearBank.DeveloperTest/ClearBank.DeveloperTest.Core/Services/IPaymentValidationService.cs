using ClearBank.DeveloperTest.Core.UseCases.Payment.MakePayment;

namespace ClearBank.DeveloperTest.Core.Services
{
    public interface IPaymentValidationService
    {
        MakePaymentResult ValidatePayment(Models.Account creditorAccount, Models.Account debtorAccount, MakePaymentRequest request);
    }
}
