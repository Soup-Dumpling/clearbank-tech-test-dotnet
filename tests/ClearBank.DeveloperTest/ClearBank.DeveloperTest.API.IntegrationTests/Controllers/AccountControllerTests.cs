using Alba;
using ClearBank.DeveloperTest.API.IntegrationTests.Extentions;
using ClearBank.DeveloperTest.Core.Enums;
using FizzWare.NBuilder;
using System.Threading.Tasks;
using Xunit;

namespace ClearBank.DeveloperTest.API.IntegrationTests.Controllers
{
    [Collection("Integration")]
    public class AccountControllerTests
    {
        private readonly IAlbaHost host;

        public AccountControllerTests(AppFixture fixture)
        {
            host = fixture.Host;
        }

        [Fact]
        public async Task GetAccount()
        {
            // Arrange
            var fakeAccounts = Builder<Core.Models.Account>.CreateListOfSize(3)
                .TheFirst(1)
                .Do(x =>
                {
                    x.AccountNumber = "51968542";
                    x.Balance = 26489.32M;
                    x.Status = AccountStatus.Live;
                    x.AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps;
                })
                .TheNext(1)
                .Do(x =>
                {
                    x.AccountNumber = "88879090";
                    x.Balance = 6872.48M;
                    x.Status = AccountStatus.InboundPaymentsOnly;
                    x.AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps;
                })
                .TheLast(1)
                .Do(x =>
                {
                    x.AccountNumber = "07815814";
                    x.Balance = 19344.50M;
                    x.Status = AccountStatus.Disabled;
                    x.AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps;
                })
                .Build();

            host.WithEmptyDatabase(async context =>
            {
                context.Accounts.AddRange(fakeAccounts);
                await context.SaveChangesAsync();
            });

            // Act
            var response = await host.Scenario(_ =>
            {
                _.Get.Url($"/api/account/{fakeAccounts[0].AccountNumber}");
                _.StatusCodeShouldBeOk();
            });

            // Assert
            var result = response.ReadAsJson<Core.Models.Account>();
            Assert.Equal(result.AccountNumber, fakeAccounts[0].AccountNumber);
            Assert.Equal(result.Balance, fakeAccounts[0].Balance);
            Assert.Equal(result.Status, fakeAccounts[0].Status);
            Assert.Equal(result.AllowedPaymentSchemes, fakeAccounts[0].AllowedPaymentSchemes);
        }
    }
}
