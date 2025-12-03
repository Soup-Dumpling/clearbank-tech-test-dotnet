using ClearBank.DeveloperTest.Core.Enums;
using ClearBank.DeveloperTest.Core.Services;
using ClearBank.DeveloperTest.Core.UseCases.Account.GetAccount;
using ClearBank.DeveloperTest.Core.UseCases.Payment.MakePayment;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ClearBank.DeveloperTest.Core.UnitTests.UseCases.Payment
{
    public class PaymentServiceUnitTests
    {
        private readonly PaymentService paymentService;
        private readonly IGetAccountRepository mockGetAccountRepository = Substitute.For<IGetAccountRepository>();
        private readonly IMakePaymentRepository mockMakePaymentRepository = Substitute.For<IMakePaymentRepository>();
        private readonly IPaymentValidationService mockPaymentValidationService = Substitute.For<IPaymentValidationService>();

        public PaymentServiceUnitTests()
        {
            paymentService = new PaymentService(mockGetAccountRepository, mockMakePaymentRepository, mockPaymentValidationService);
        }

        [Fact]
        public async Task ValidMakePayment()
        {
            // Arrange
            var request = new MakePaymentRequest("34279741", "76831584", 10000.00M, DateTime.UtcNow, PaymentScheme.FasterPayments);
            var creditorAccountInitialBalance = 35741.11M;
            var debtorAccountInitialBalance = 19237.59M;
            var creditorAccount = new Models.Account()
            {
                AccountNumber = request.CreditorAccountNumber,
                Balance = creditorAccountInitialBalance,
                Status = AccountStatus.Live,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs,
            };
            var debtorAccount = new Models.Account()
            {
                AccountNumber = request.DebtorAccountNumber,
                Balance = debtorAccountInitialBalance,
                Status = AccountStatus.Live,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps,
            };
            mockGetAccountRepository.GetAccountByAccountNumberAsync(Arg.Any<string>()).Returns(Task.FromResult(creditorAccount), Task.FromResult(debtorAccount));
            mockPaymentValidationService.ValidatePayment(Arg.Any<Models.Account>(), Arg.Any<Models.Account>(), Arg.Any<MakePaymentRequest>()).Returns(new MakePaymentResult() { Success = true });

            // Act
            var result = await paymentService.MakePayment(request);

            // Assert
            Assert.True(result.Success);
            await mockGetAccountRepository.Received().GetAccountByAccountNumberAsync(request.CreditorAccountNumber);
            await mockGetAccountRepository.Received().GetAccountByAccountNumberAsync(request.DebtorAccountNumber);
            mockPaymentValidationService.Received().ValidatePayment(Arg.Is<Models.Account>(a => a.Balance == creditorAccount.Balance), Arg.Is<Models.Account>(a => a.Balance == debtorAccount.Balance), request);
            await mockMakePaymentRepository.Received().UpdateAccounts();
            Assert.Equal(creditorAccountInitialBalance + request.Amount, creditorAccount.Balance);
            Assert.Equal(debtorAccountInitialBalance - request.Amount, debtorAccount.Balance);
        }

        [Fact]
        public async Task CreditorAccountNotFound()
        {
            // Arrange
            var request = new MakePaymentRequest("34279741", "76831584", 10000.00M, DateTime.UtcNow, PaymentScheme.FasterPayments);
            var debtorAccount = new Models.Account()
            {
                AccountNumber = request.DebtorAccountNumber,
                Balance = 19237.59M,
                Status = AccountStatus.Live,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps,
            };
            mockGetAccountRepository.GetAccountByAccountNumberAsync(Arg.Any<string>()).Returns(Task.FromResult(null as Models.Account), Task.FromResult(debtorAccount));

            // Act
            var result = await paymentService.MakePayment(request);

            // Assert
            Assert.False(result.Success);
            await mockGetAccountRepository.Received().GetAccountByAccountNumberAsync(request.CreditorAccountNumber);
            await mockGetAccountRepository.Received().GetAccountByAccountNumberAsync(request.DebtorAccountNumber);
            mockPaymentValidationService.DidNotReceive().ValidatePayment(Arg.Any<Models.Account>(), Arg.Any<Models.Account>(), Arg.Any<MakePaymentRequest>());
            await mockMakePaymentRepository.DidNotReceive().UpdateAccounts();
        }

        [Fact]
        public async Task DebtorAccountNotFound()
        {
            // Arrange
            var request = new MakePaymentRequest("34279741", "76831584", 10000.00M, DateTime.UtcNow, PaymentScheme.FasterPayments);
            var creditorAccount = new Models.Account()
            {
                AccountNumber = request.CreditorAccountNumber,
                Balance = 35741.11M,
                Status = AccountStatus.Live,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs,
            };
            mockGetAccountRepository.GetAccountByAccountNumberAsync(Arg.Any<string>()).Returns(Task.FromResult(creditorAccount), Task.FromResult(null as Models.Account));

            // Act
            var result = await paymentService.MakePayment(request);

            // Assert
            Assert.False(result.Success);
            await mockGetAccountRepository.Received().GetAccountByAccountNumberAsync(request.CreditorAccountNumber);
            await mockGetAccountRepository.Received().GetAccountByAccountNumberAsync(request.DebtorAccountNumber);
            mockPaymentValidationService.DidNotReceive().ValidatePayment(Arg.Any<Models.Account>(), Arg.Any<Models.Account>(), Arg.Any<MakePaymentRequest>());
            await mockMakePaymentRepository.DidNotReceive().UpdateAccounts();
        }

        [Fact]
        public async Task BothAccountsNotFound()
        {
            // Arrange
            var request = new MakePaymentRequest("34279741", "76831584", 10000.00M, DateTime.UtcNow, PaymentScheme.FasterPayments);
            mockGetAccountRepository.GetAccountByAccountNumberAsync(Arg.Any<string>()).Returns(Task.FromResult(null as Models.Account), Task.FromResult(null as Models.Account));

            // Act
            var result = await paymentService.MakePayment(request);

            // Assert
            Assert.False(result.Success);
            await mockGetAccountRepository.Received().GetAccountByAccountNumberAsync(request.CreditorAccountNumber);
            await mockGetAccountRepository.Received().GetAccountByAccountNumberAsync(request.DebtorAccountNumber);
            mockPaymentValidationService.DidNotReceive().ValidatePayment(Arg.Any<Models.Account>(), Arg.Any<Models.Account>(), Arg.Any<MakePaymentRequest>());
            await mockMakePaymentRepository.DidNotReceive().UpdateAccounts();
        }

        [Fact]
        public async Task PaymentValidationFailed()
        {
            // Arrange
            var request = new MakePaymentRequest("34279741", "76831584", 20000.00M, DateTime.UtcNow, PaymentScheme.Chaps);
            var creditorAccountInitialBalance = 35741.11M;
            var debtorAccountInitialBalance = 19237.59M;
            var creditorAccount = new Models.Account()
            {
                AccountNumber = request.CreditorAccountNumber,
                Balance = creditorAccountInitialBalance,
                Status = AccountStatus.Live,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs,
            };
            var debtorAccount = new Models.Account()
            {
                AccountNumber = request.DebtorAccountNumber,
                Balance = debtorAccountInitialBalance,
                Status = AccountStatus.Live,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps,
            };
            mockGetAccountRepository.GetAccountByAccountNumberAsync(Arg.Any<string>()).Returns(Task.FromResult(creditorAccount), Task.FromResult(debtorAccount));
            mockPaymentValidationService.ValidatePayment(Arg.Any<Models.Account>(), Arg.Any<Models.Account>(), Arg.Any<MakePaymentRequest>()).Returns(new MakePaymentResult() { Success = false });

            // Act
            var result = await paymentService.MakePayment(request);

            // Assert
            Assert.False(result.Success);
            await mockGetAccountRepository.Received().GetAccountByAccountNumberAsync(request.CreditorAccountNumber);
            await mockGetAccountRepository.Received().GetAccountByAccountNumberAsync(request.DebtorAccountNumber);
            mockPaymentValidationService.Received().ValidatePayment(Arg.Is<Models.Account>(a => a.Balance == creditorAccount.Balance), Arg.Is<Models.Account>(a => a.Balance == debtorAccount.Balance), request);
            await mockMakePaymentRepository.DidNotReceive().UpdateAccounts();
            Assert.Equal(creditorAccountInitialBalance, creditorAccount.Balance);
            Assert.Equal(debtorAccountInitialBalance, debtorAccount.Balance);
        }
    }
}
