using FluentValidation;

namespace ClearBank.DeveloperTest.Core.UseCases.Account.GetAccount
{
    public class GetAccountQueryValidator : AbstractValidator<GetAccountQuery>
    {
        public GetAccountQueryValidator() 
        {
            RuleFor(x => x.AccountNumber).NotEmpty();
        }
    }
}
