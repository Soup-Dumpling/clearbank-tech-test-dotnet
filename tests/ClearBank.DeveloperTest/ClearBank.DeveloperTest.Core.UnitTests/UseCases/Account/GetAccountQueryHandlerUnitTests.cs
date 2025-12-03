using ClearBank.DeveloperTest.Core.Enums;
using ClearBank.DeveloperTest.Core.Exceptions;
using ClearBank.DeveloperTest.Core.UseCases.Account.GetAccount;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClearBank.DeveloperTest.Core.UnitTests.UseCases.Account
{
    public class GetAccountQueryHandlerUnitTests
    {
        private readonly GetAccountQueryHandler getAccountQueryHandler;
        private readonly IGetAccountRepository mockGetAccountRepository = Substitute.For<IGetAccountRepository>();

        public GetAccountQueryHandlerUnitTests() 
        {
            getAccountQueryHandler = new GetAccountQueryHandler(mockGetAccountRepository);
        }

        [Fact]
        public async Task ValidQuery()
        {
            // Arrange
            var accountNumber = "accountNumber";
            var query = new GetAccountQuery(accountNumber);
            var account = new Models.Account()
            {
                AccountNumber = accountNumber,
                Balance = 1674.50M,
                Status = AccountStatus.Live,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps
            };
            mockGetAccountRepository.GetAccountByAccountNumberAsync(Arg.Any<string>()).Returns(Task.FromResult(account));

            // Act
            var result = await getAccountQueryHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Models.Account>(result);
            await mockGetAccountRepository.Received().GetAccountByAccountNumberAsync(query.AccountNumber);
        }

        [Fact]
        public async Task AccountNotFound()
        {
            // Arrange
            var accountNumber = "accountNumber";
            var query = new GetAccountQuery(accountNumber);
            mockGetAccountRepository.GetAccountByAccountNumberAsync(Arg.Any<string>()).ReturnsNull();

            // Act
            await Assert.ThrowsAsync<NotFoundException>(async () => await getAccountQueryHandler.Handle(query, CancellationToken.None));
            await mockGetAccountRepository.Received().GetAccountByAccountNumberAsync(accountNumber);
        }
    }
}
