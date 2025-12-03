using ClearBank.DeveloperTest.Core.UseCases.Account.GetAccount;
using FluentValidation.TestHelper;
using System.Threading.Tasks;
using Xunit;

namespace ClearBank.DeveloperTest.Core.UnitTests.UseCases.Account
{
    public class GetAccountQueryValidatorUnitTests
    {
        private readonly GetAccountQueryValidator validator;

        public GetAccountQueryValidatorUnitTests()
        {
            validator = new GetAccountQueryValidator();
        }

        [Fact]
        public async Task ValidQuery()
        {
            var query = new GetAccountQuery("accountNumber");
            var result = await validator.TestValidateAsync(query);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task InvalidQuery()
        {
            var query = new GetAccountQuery(string.Empty);
            var result = await validator.TestValidateAsync(query);
            result.ShouldHaveValidationErrorFor(x => x.AccountNumber);
        }
    }
}
