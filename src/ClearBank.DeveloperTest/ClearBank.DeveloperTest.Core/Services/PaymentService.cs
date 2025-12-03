using ClearBank.DeveloperTest.Core.Exceptions;
using ClearBank.DeveloperTest.Core.UseCases.Account.GetAccount;
using ClearBank.DeveloperTest.Core.UseCases.Payment.MakePayment;
using System.Threading.Tasks;

namespace ClearBank.DeveloperTest.Core.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IGetAccountRepository getAccountRepository;
        private readonly IMakePaymentRepository makePaymentRepository;
        private readonly IPaymentValidationService paymentValidationService;

        public PaymentService(IGetAccountRepository getAccountRepository, IMakePaymentRepository makePaymentRepository, IPaymentValidationService paymentValidationService)
        {
            this.getAccountRepository = getAccountRepository;
            this.makePaymentRepository = makePaymentRepository;
            this.paymentValidationService = paymentValidationService;
        }

        public async Task<MakePaymentResult> MakePayment(MakePaymentRequest request)
        {
            // Typically, payment transactions flow from the debtor to the creditor
            // Check that both the creditor and debtor accounts exist
            var result = new MakePaymentResult();
            var creditorAccount = await getAccountRepository.GetAccountByAccountNumberAsync(request.CreditorAccountNumber);
            var debtorAccount = await getAccountRepository.GetAccountByAccountNumberAsync(request.DebtorAccountNumber);

            if (creditorAccount == null || debtorAccount == null)
            {
                //throw new NotFoundException();
                result.Success = false;
                return result;
            }

            // Check if the payment is valid
            result = paymentValidationService.ValidatePayment(creditorAccount, debtorAccount, request);

            // Valid payment after the validation checks
            if (result.Success) 
            {
                // Update the debtor and creditor account balance
                creditorAccount.Balance += request.Amount;
                debtorAccount.Balance -= request.Amount;
                await makePaymentRepository.UpdateAccounts();
            }

            return result;
        }
    }
}
