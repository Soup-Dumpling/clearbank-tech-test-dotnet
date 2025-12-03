using ClearBank.DeveloperTest.Core.Exceptions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ClearBank.DeveloperTest.Core.UseCases.Account.GetAccount
{
    public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, Models.Account>
    {
        private readonly IGetAccountRepository getAccountRepository;

        public GetAccountQueryHandler(IGetAccountRepository getAccountRepository) 
        {
            this.getAccountRepository = getAccountRepository;
        }

        public async Task<Models.Account> Handle(GetAccountQuery request, CancellationToken cancellationToken)
        {
            var result = await getAccountRepository.GetAccountByAccountNumberAsync(request.AccountNumber);
            return result ?? throw new NotFoundException();
        }
    }
}
