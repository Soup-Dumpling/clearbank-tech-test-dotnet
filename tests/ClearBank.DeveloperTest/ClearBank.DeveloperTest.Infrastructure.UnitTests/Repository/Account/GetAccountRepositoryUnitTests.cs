using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using ClearBank.DeveloperTest.Core.Enums;
using ClearBank.DeveloperTest.Infrastructure.Repository.Account;
using System.Threading.Tasks;
using Xunit;

namespace ClearBank.DeveloperTest.Infrastructure.UnitTests.Repository.Account
{
    public class GetAccountRepositoryUnitTests
    {
        private readonly ClearBankContext context;
        private readonly GetAccountRepository getAccountRepository;

        public GetAccountRepositoryUnitTests()
        {
            context = Helpers.InMemoryContextHelper.GetContext();
            getAccountRepository = new GetAccountRepository(context);
        }

        [Fact]
        public async Task ValidGetAccountByAccountNumber()
        {
            // Arrange
            var fakeAccount = Builder<Core.Models.Account>.CreateNew()
                .Do(x =>
                {
                    x.AccountNumber = "68154674";
                    x.Balance = 16485.39M;
                    x.Status = AccountStatus.Live;
                    x.AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps;
                })
                .Build();
            context.Accounts.Add(fakeAccount);
            await context.SaveChangesAsync();

            // Act
            var result = await getAccountRepository.GetAccountByAccountNumberAsync(fakeAccount.AccountNumber);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Core.Models.Account>(result);
            var count = await context.Accounts.CountAsync();
            Assert.Equal(fakeAccount.AccountNumber, result.AccountNumber);
            Assert.Equal(fakeAccount.Balance, result.Balance);
            Assert.Equal(fakeAccount.Status, result.Status);
            Assert.Equal(fakeAccount.AllowedPaymentSchemes, result.AllowedPaymentSchemes);
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task InvalidGetAccountByInvalidAccountNumber()
        {
            // Arrange
            var fakeAccountNumber = "invalidAccountNumber";

            // Act
            var result = await getAccountRepository.GetAccountByAccountNumberAsync(fakeAccountNumber);

            // Assert
            Assert.Null(result);
        }
    }
}
