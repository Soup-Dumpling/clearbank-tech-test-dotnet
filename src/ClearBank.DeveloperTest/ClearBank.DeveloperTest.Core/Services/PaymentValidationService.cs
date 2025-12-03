using ClearBank.DeveloperTest.Core.Enums;
using ClearBank.DeveloperTest.Core.Exceptions;
using ClearBank.DeveloperTest.Core.UseCases.Payment.MakePayment;
using System.Collections.Generic;

namespace ClearBank.DeveloperTest.Core.Services
{
    public class PaymentValidationService : IPaymentValidationService
    {
        public MakePaymentResult ValidatePayment(Models.Account creditorAccount, Models.Account debtorAccount, MakePaymentRequest request)
        {
            //var errors = new Dictionary<string, string[]>();

            // Creditor account must be Live or InboundPaymentsOnly
            bool validCreditorAccount = creditorAccount.Status == AccountStatus.Live || creditorAccount.Status == AccountStatus.InboundPaymentsOnly;

            // Debtor account must be Live
            bool validDebtorAccount = debtorAccount.Status == AccountStatus.Live;

            if (!validCreditorAccount)
            {
                //errors.Add("creditorAccount.Status", ["Creditor account status is Disabled."]);
                //throw new ValidationException(errors);
                return new MakePaymentResult() { Success = false };
            }

            if (!validDebtorAccount)
            {
                //errors.Add("debtorAccount.Status", ["Debtor account status is not Live."]);
                //throw new ValidationException(errors);
                return new MakePaymentResult() { Success = false };
            }

            // Debtor account must have enough balance to pay the payment amount
            if (debtorAccount.Balance < request.Amount)
            {
                //errors.Add("debtorAccount.Balance", ["Debtor account balance is less than the payment amount."]);
                //throw new ValidationException(errors);
                return new MakePaymentResult() { Success = false };
            }

            // Ensure that both the debtor account and creditor account support the requested payment scheme
            if (!SupportsRequestedPaymentScheme(creditorAccount, request.PaymentScheme) || !SupportsRequestedPaymentScheme(debtorAccount, request.PaymentScheme))
            {
                return new MakePaymentResult() { Success = false};
            }

            return new MakePaymentResult() { Success =  true };
        }

        private static bool SupportsRequestedPaymentScheme(Models.Account account, PaymentScheme paymentScheme)
        {
            return paymentScheme switch
            {
                PaymentScheme.FasterPayments => account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments),
                PaymentScheme.Bacs => account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs),
                PaymentScheme.Chaps => account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps),
                _ => false,
            };
        }
    }
}
