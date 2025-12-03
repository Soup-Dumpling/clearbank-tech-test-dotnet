using Alba;
using ClearBank.DeveloperTest.API.IntegrationTests.Extentions;
using ClearBank.DeveloperTest.API.Models.Payment;
using ClearBank.DeveloperTest.Core.Enums;
using ClearBank.DeveloperTest.Core.UseCases.Payment.MakePayment;
using FizzWare.NBuilder;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ClearBank.DeveloperTest.API.IntegrationTests.Controllers
{
    [Collection("Integration")]
    public class PaymentControllerTests
    {
        private readonly IAlbaHost host;

        public PaymentControllerTests(AppFixture fixture)
        {
            host = fixture.Host;
        }

        // Creditor Account Status, Debtor Account Status, Creditor Account Allowed Payment Schemes, Debtor Account Allowed Payment Schemes, Request Payment Scheme, Request Payment Amount
        public class InvalidMakePaymentScenarios : TheoryData<AccountStatus, AccountStatus, AllowedPaymentSchemes, AllowedPaymentSchemes, PaymentScheme, decimal>
        {
            public InvalidMakePaymentScenarios()
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
        public async Task ValidMakePayment()
        {
            // Arrange
            var creditorAccount = Builder<Core.Models.Account>.CreateNew()
                .Do(x =>
                {
                    x.AccountNumber = "88879090";
                    x.Balance = 6872.48M;
                    x.Status = AccountStatus.InboundPaymentsOnly;
                    x.AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps;
                })
                .Build();
            var debtorAccount = Builder<Core.Models.Account>.CreateNew()
                .Do(x =>
                {
                    x.AccountNumber = "51968542";
                    x.Balance = 40489.32M;
                    x.Status = AccountStatus.Live;
                    x.AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps;
                })
                .Build();

            host.WithEmptyDatabase(async context =>
            {
                context.Accounts.Add(creditorAccount);
                context.Accounts.Add(debtorAccount);
                await context.SaveChangesAsync();
            });

            var request = Builder<MakePaymentRequestPayload>.CreateNew()
                .Do(x =>
                {
                    x.CreditorAccountNumber = creditorAccount.AccountNumber;
                    x.DebtorAccountNumber = debtorAccount.AccountNumber;
                    x.Amount = 10000.00M;
                    x.PaymentDate = DateTime.UtcNow;
                    x.PaymentScheme = PaymentScheme.Bacs;
                })
                .Build();

            // Act
            var response = await host.Scenario(_ =>
            {
                _.Put.Json(request).ToUrl($"/api/payment");
                _.StatusCodeShouldBeOk();
            });

            // Assert
            var result = response.ReadAsJson<MakePaymentResult>();
            Assert.True(result.Success);

            var creditorAccountResponse = await host.Scenario(_ =>
            {
                _.Get.Url($"/api/account/{creditorAccount.AccountNumber}");
                _.StatusCodeShouldBeOk();
            });
            var creditorAccountResult = creditorAccountResponse.ReadAsJson<Core.Models.Account>();
            Assert.Equal(creditorAccount.Balance + request.Amount, creditorAccountResult.Balance);

            var debtorAccountResponse = await host.Scenario(_ =>
            {
                _.Get.Url($"/api/account/{debtorAccount.AccountNumber}");
                _.StatusCodeShouldBeOk();
            });
            var debtorAccountResult = debtorAccountResponse.ReadAsJson<Core.Models.Account>();
            Assert.Equal(debtorAccount.Balance - request.Amount, debtorAccountResult.Balance);
        }

        [Fact]
        public async Task InvalidMakePayment_CreditorAccountNotFound()
        {
            // Arrange
            var debtorAccount = Builder<Core.Models.Account>.CreateNew()
                .Do(x =>
                {
                    x.AccountNumber = "51968542";
                    x.Balance = 40489.32M;
                    x.Status = AccountStatus.Live;
                    x.AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps;
                })
                .Build();

            host.WithEmptyDatabase(async context =>
            {
                context.Accounts.Add(debtorAccount);
                await context.SaveChangesAsync();
            });

            var request = Builder<MakePaymentRequestPayload>.CreateNew()
                .Do(x =>
                {
                    x.CreditorAccountNumber = "88879090";
                    x.DebtorAccountNumber = debtorAccount.AccountNumber;
                    x.Amount = 10000.00M;
                    x.PaymentDate = DateTime.UtcNow;
                    x.PaymentScheme = PaymentScheme.Bacs;
                })
                .Build();

            // Act
            var response = await host.Scenario(_ =>
            {
                _.Put.Json(request).ToUrl($"/api/payment");
                _.StatusCodeShouldBeOk();
            });

            // Assert
            var result = response.ReadAsJson<MakePaymentResult>();
            Assert.False(result.Success);

            // Verify the existing debtor account balance is unchanged
            var debtorAccountResponse = await host.Scenario(_ =>
            {
                _.Get.Url($"/api/account/{debtorAccount.AccountNumber}");
                _.StatusCodeShouldBeOk();
            });
            var debtorAccountResult = debtorAccountResponse.ReadAsJson<Core.Models.Account>();
            Assert.Equal(debtorAccount.Balance, debtorAccountResult.Balance);
        }

        [Fact]
        public async Task InvalidMakePayment_DebtorAccountNotFound()
        {
            // Arrange
            var creditorAccount = Builder<Core.Models.Account>.CreateNew()
                .Do(x =>
                {
                    x.AccountNumber = "88879090";
                    x.Balance = 6872.48M;
                    x.Status = AccountStatus.InboundPaymentsOnly;
                    x.AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps;
                })
                .Build();

            host.WithEmptyDatabase(async context =>
            {
                context.Accounts.Add(creditorAccount);
                await context.SaveChangesAsync();
            });

            var request = Builder<MakePaymentRequestPayload>.CreateNew()
                .Do(x =>
                {
                    x.CreditorAccountNumber = creditorAccount.AccountNumber;
                    x.DebtorAccountNumber = "51968542";
                    x.Amount = 10000.00M;
                    x.PaymentDate = DateTime.UtcNow;
                    x.PaymentScheme = PaymentScheme.Bacs;
                })
                .Build();

            // Act
            var response = await host.Scenario(_ =>
            {
                _.Put.Json(request).ToUrl($"/api/payment");
                _.StatusCodeShouldBeOk();
            });

            // Assert
            var result = response.ReadAsJson<MakePaymentResult>();
            Assert.False(result.Success);

            // Verify the existing creditor account balance is unchanged
            var creditorAccountResponse = await host.Scenario(_ =>
            {
                _.Get.Url($"/api/account/{creditorAccount.AccountNumber}");
                _.StatusCodeShouldBeOk();
            });
            var creditorAccountResult = creditorAccountResponse.ReadAsJson<Core.Models.Account>();
            Assert.Equal(creditorAccount.Balance, creditorAccountResult.Balance);
        }

        [Fact]
        public async Task InvalidMakePayment_BothAccountsNotFound()
        {
            // Arrange
            host.WithEmptyDatabase(async context =>
            {
                await context.SaveChangesAsync();
            });

            var request = Builder<MakePaymentRequestPayload>.CreateNew()
                .Do(x =>
                {
                    x.CreditorAccountNumber = "88879090";
                    x.DebtorAccountNumber = "51968542";
                    x.Amount = 10000.00M;
                    x.PaymentDate = DateTime.UtcNow;
                    x.PaymentScheme = PaymentScheme.Bacs;
                })
                .Build();

            // Act
            var response = await host.Scenario(_ =>
            {
                _.Put.Json(request).ToUrl($"/api/payment");
                _.StatusCodeShouldBeOk();
            });

            // Assert
            var result = response.ReadAsJson<MakePaymentResult>();
            Assert.False(result.Success);
        }

        [Theory]
        [ClassData(typeof(InvalidMakePaymentScenarios))]
        public async Task InvalidMakePayment_InvalidPaymentDetailValidation(AccountStatus creditorAccountStatus, AccountStatus debtorAccountStatus, AllowedPaymentSchemes creditorAccountPaymentSchemes, AllowedPaymentSchemes debtorAccountPaymentSchemes, PaymentScheme requestPaymentScheme, decimal requestPaymentAmount)
        {
            // Arrange
            var creditorAccount = Builder<Core.Models.Account>.CreateNew()
                .Do(x =>
                {
                    x.AccountNumber = "88879090";
                    x.Balance = 6872.48M;
                    x.Status = creditorAccountStatus;
                    x.AllowedPaymentSchemes = creditorAccountPaymentSchemes;
                })
                .Build();
            var debtorAccount = Builder<Core.Models.Account>.CreateNew()
                .Do(x =>
                {
                    x.AccountNumber = "51968542";
                    x.Balance = 40489.32M;
                    x.Status = debtorAccountStatus;
                    x.AllowedPaymentSchemes = debtorAccountPaymentSchemes;
                })
                .Build();

            host.WithEmptyDatabase(async context =>
            {
                context.Accounts.Add(creditorAccount);
                context.Accounts.Add(debtorAccount);
                await context.SaveChangesAsync();
            });

            var request = Builder<MakePaymentRequestPayload>.CreateNew()
                .Do(x =>
                {
                    x.CreditorAccountNumber = creditorAccount.AccountNumber;
                    x.DebtorAccountNumber = debtorAccount.AccountNumber;
                    x.Amount = requestPaymentAmount;
                    x.PaymentDate = DateTime.UtcNow;
                    x.PaymentScheme = requestPaymentScheme;
                })
                .Build();

            // Act
            var response = await host.Scenario(_ =>
            {
                _.Put.Json(request).ToUrl($"/api/payment");
                _.StatusCodeShouldBeOk();
            });

            // Assert
            var result = response.ReadAsJson<MakePaymentResult>();
            Assert.False(result.Success);

            // Verify the existing creditor account balance is unchanged
            var creditorAccountResponse = await host.Scenario(_ =>
            {
                _.Get.Url($"/api/account/{creditorAccount.AccountNumber}");
                _.StatusCodeShouldBeOk();
            });
            var creditorAccountResult = creditorAccountResponse.ReadAsJson<Core.Models.Account>();
            Assert.Equal(creditorAccount.Balance, creditorAccountResult.Balance);

            // Verify the existing debtor account balance is unchanged
            var debtorAccountResponse = await host.Scenario(_ =>
            {
                _.Get.Url($"/api/account/{debtorAccount.AccountNumber}");
                _.StatusCodeShouldBeOk();
            });
            var debtorAccountResult = debtorAccountResponse.ReadAsJson<Core.Models.Account>();
            Assert.Equal(debtorAccount.Balance, debtorAccountResult.Balance);
        }
    }
}
