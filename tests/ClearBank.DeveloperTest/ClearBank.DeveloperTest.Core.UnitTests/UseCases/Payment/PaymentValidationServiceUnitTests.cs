using ClearBank.DeveloperTest.Core.Enums;
using ClearBank.DeveloperTest.Core.Services;
using ClearBank.DeveloperTest.Core.UseCases.Payment.MakePayment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ClearBank.DeveloperTest.Core.UnitTests.UseCases.Payment
{
    public class PaymentValidationServiceUnitTests
    {
        private readonly PaymentValidationService paymentValidationService;

        public PaymentValidationServiceUnitTests()
        {
            paymentValidationService = new PaymentValidationService();
        }

        // Creditor Account Status, Debtor Account Status, Creditor Account Allowed Payment Schemes, Debtor Account Allowed Payment Schemes, Request Payment Scheme, Request Payment Amount
        public class InvalidValidatePaymentScenarios : TheoryData<AccountStatus, AccountStatus, AllowedPaymentSchemes, AllowedPaymentSchemes, PaymentScheme, decimal>
        {
            public InvalidValidatePaymentScenarios() 
            {
                Add(AccountStatus.Disabled, AccountStatus.Live, AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps, AllowedPaymentSchemes.Chaps, PaymentScheme.Chaps, 10000.00M); // Scenario: Invalid Credit Account Status (Disabled)
                Add(AccountStatus.InboundPaymentsOnly, AccountStatus.Disabled, AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs, AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps, PaymentScheme.FasterPayments, 10000.00M); // Scenario: Invalid Debtor Account Status (Disabled)
                Add(AccountStatus.Live, AccountStatus.InboundPaymentsOnly, AllowedPaymentSchemes.Bacs, AllowedPaymentSchemes.Bacs, PaymentScheme.Bacs, 10000.00M); // Scenario: Invalid Debtor Account Status (InboundPaymentsOnly)
                Add(AccountStatus.InboundPaymentsOnly, AccountStatus.Live, AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps, AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps, PaymentScheme.Bacs, 50000.00M); // Scenario: Insufficient Balance in Debtor Account
                Add(AccountStatus.Live, AccountStatus.Live, AllowedPaymentSchemes.Chaps, AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs, PaymentScheme.Bacs, 10000.00M); // Scenario: Creditor Account Payment Scheme Mismatch (e.g., trying to BACS to a CHAPS-only creditor)
                Add(AccountStatus.Live, AccountStatus.Live, AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps, AllowedPaymentSchemes.Bacs, PaymentScheme.FasterPayments, 10000.00M); // Scenario: Debtor Account Payment Scheme Mismatch (e.g., trying to FasterPayments from a BACS-only debtor)
                Add(AccountStatus.InboundPaymentsOnly, AccountStatus.Live, 0, AllowedPaymentSchemes.FasterPayments, PaymentScheme.FasterPayments, 10000.00M); // Scenario: Creditor Account not subscribed to any Payment Schemes
                Add(AccountStatus.Live, AccountStatus.Live, AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps, 0, PaymentScheme.Chaps, 10000.00M); // Scenario: Debtor Account not subscribed to any Payment Schemes
            }
        }

        [Fact]
        public void ValidatePayment_WhenValid_ReturnsSuccess()
        {
            // Arrange
            var creditorAccount = new Models.Account()
            {
                AccountNumber = "18537860",
                Balance = 26397.65M,
                Status = AccountStatus.InboundPaymentsOnly,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps,
            };
            var debtorAccount = new Models.Account()
            {
                AccountNumber = "97881130",
                Balance = 14368.86M,
                Status = AccountStatus.Live,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps,
            };
            var request = new MakePaymentRequest(creditorAccount.AccountNumber, debtorAccount.AccountNumber, 5500.00M, DateTime.UtcNow, PaymentScheme.Chaps);

            // Act
            var result = paymentValidationService.ValidatePayment(creditorAccount, debtorAccount, request);

            // Assert
            Assert.True(result.Success);
        }

        [Theory]
        [ClassData(typeof(InvalidValidatePaymentScenarios))]
        public void ValidatePayment_WhenInvalid_ReturnsFailure(AccountStatus creditorAccountStatus, AccountStatus debtorAccountStatus, AllowedPaymentSchemes creditorAccountPaymentSchemes, AllowedPaymentSchemes debtorAccountPaymentSchemes, PaymentScheme requestPaymentScheme, decimal requestPaymentAmount)
        {
            // Arrange
            var creditorAccount = new Models.Account()
            {
                AccountNumber = "18537860",
                Balance = 26397.65M,
                Status = creditorAccountStatus,
                AllowedPaymentSchemes = creditorAccountPaymentSchemes,
            };
            var debtorAccount = new Models.Account()
            {
                AccountNumber = "97881130",
                Balance = 14368.86M,
                Status = debtorAccountStatus,
                AllowedPaymentSchemes = debtorAccountPaymentSchemes,
            };
            var request = new MakePaymentRequest(creditorAccount.AccountNumber, debtorAccount.AccountNumber, requestPaymentAmount, DateTime.UtcNow, requestPaymentScheme);

            // Act
            var result = paymentValidationService.ValidatePayment(creditorAccount, debtorAccount, request);

            // Assert
            Assert.False(result.Success);
        }
    }
}
