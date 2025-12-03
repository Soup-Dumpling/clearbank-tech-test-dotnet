using MediatR;

namespace ClearBank.DeveloperTest.Core.UseCases.Account.GetAccount
{
    public class GetAccountQuery : IRequest<Models.Account>
    {
        public string AccountNumber { get; set; }

        public GetAccountQuery(string accountNumber)
        {
            AccountNumber = accountNumber;
        }
    }
}
