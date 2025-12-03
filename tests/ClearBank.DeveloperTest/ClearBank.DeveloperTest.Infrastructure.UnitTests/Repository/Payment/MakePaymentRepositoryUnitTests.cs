using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using ClearBank.DeveloperTest.Core.Enums;
using ClearBank.DeveloperTest.Infrastructure.Repository.Payment;
using System.Threading.Tasks;
using Xunit;

namespace ClearBank.DeveloperTest.Infrastructure.UnitTests.Repository.Payment
{
    public class MakePaymentRepositoryUnitTests
    {
        private readonly ClearBankContext context;
        private readonly MakePaymentRepository makePaymentRepository;

        public MakePaymentRepositoryUnitTests() 
        {
            context = Helpers.InMemoryContextHelper.GetContext();
            makePaymentRepository = new MakePaymentRepository(context);
        }

        [Fact]
        public async Task ValidUpdateAccounts()
        {
            // Arrange
            var paymentAmount = 500.80M;
            var fakeAccounts = Builder<Core.Models.Account>.CreateListOfSize(2)
                .TheFirst(1)
                .Do(x =>
                {
                    x.AccountNumber = "06012892";
                    x.Balance = 8715.52M;
                    x.Status = AccountStatus.InboundPaymentsOnly;
                    x.AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs;
                })
                .TheLast(1)
                .Do(x =>
                {
                    x.AccountNumber = "48791545";
                    x.Balance = 1356.80M;
                    x.Status = AccountStatus.Live;
                    x.AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments;
                })
                .Build();
            context.Accounts.AddRange(fakeAccounts);
            await context.SaveChangesAsync();

            var expectedCreditorAccountNewBalance = fakeAccounts[0].Balance + paymentAmount;
            var expectedDebtorAccountNewBalance = fakeAccounts[1].Balance - paymentAmount;

            fakeAccounts[0].Balance = expectedCreditorAccountNewBalance;
            fakeAccounts[1].Balance = expectedDebtorAccountNewBalance;

            // Act
            await makePaymentRepository.UpdateAccounts();

            // Assert
            var creditorAccount = await context.Accounts.FindAsync(fakeAccounts[0].AccountNumber);
            var debtorAccount = await context.Accounts.FindAsync(fakeAccounts[1].AccountNumber);
            var count = await context.Accounts.CountAsync();
            Assert.Equal(fakeAccounts[0].AccountNumber, creditorAccount.AccountNumber);
            Assert.Equal(expectedCreditorAccountNewBalance, creditorAccount.Balance);
            Assert.Equal(fakeAccounts[0].Status, creditorAccount.Status);
            Assert.Equal(fakeAccounts[0].AllowedPaymentSchemes, creditorAccount.AllowedPaymentSchemes);
            Assert.Equal(fakeAccounts[1].AccountNumber, debtorAccount.AccountNumber);
            Assert.Equal(expectedDebtorAccountNewBalance, debtorAccount.Balance);
            Assert.Equal(fakeAccounts[1].Status, debtorAccount.Status);
            Assert.Equal(fakeAccounts[1].AllowedPaymentSchemes, debtorAccount.AllowedPaymentSchemes);
            Assert.Equal(2, count);
        }
    }
}
