using FluentValidation;
using System;

namespace ClearBank.DeveloperTest.Core.UseCases.Payment.MakePayment
{
    public class MakePaymentRequestValidator : AbstractValidator<MakePaymentRequest>
    {
        public MakePaymentRequestValidator() 
        {
            RuleFor(x => x.CreditorAccountNumber).NotEmpty();
            RuleFor(x => x.DebtorAccountNumber).NotEmpty();
            RuleFor(x => x.Amount).NotEmpty().GreaterThan(0).WithMessage("Payment amount must be greater than zero");
            RuleFor(x => x.PaymentDate).NotEmpty().GreaterThanOrEqualTo(DateTime.Today);
            RuleFor(x => x.PaymentScheme).IsInEnum();
        }
    }
}
