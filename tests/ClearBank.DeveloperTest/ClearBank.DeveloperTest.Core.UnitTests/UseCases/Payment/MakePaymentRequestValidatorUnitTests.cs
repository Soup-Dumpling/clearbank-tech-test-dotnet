using ClearBank.DeveloperTest.Core.Enums;
using ClearBank.DeveloperTest.Core.UseCases.Payment.MakePayment;
using FluentValidation.TestHelper;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ClearBank.DeveloperTest.Core.UnitTests.UseCases.Payment
{
    public class MakePaymentRequestValidatorUnitTests
    {
        private readonly MakePaymentRequestValidator validator;

        public MakePaymentRequestValidatorUnitTests() 
        {
            validator = new MakePaymentRequestValidator();
        }

        [Fact]
        public async Task ValidRequest()
        {
            var request = new MakePaymentRequest("90580035", "21912910", 250.00M, DateTime.UtcNow, PaymentScheme.FasterPayments);
            var result = await validator.TestValidateAsync(request);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task InvalidRequest()
        {
            var request = new MakePaymentRequest(string.Empty, string.Empty, 0, DateTime.UtcNow.AddDays(-1), 0);
            var result = await validator.TestValidateAsync(request);
            result.ShouldHaveValidationErrorFor(x => x.CreditorAccountNumber);
            result.ShouldHaveValidationErrorFor(x => x.DebtorAccountNumber);
            result.ShouldHaveValidationErrorFor(x => x.Amount);
            result.ShouldHaveValidationErrorFor(x => x.PaymentDate);
            result.ShouldHaveValidationErrorFor(x => x.PaymentScheme);
        }
    }
}
