using ClearBank.DeveloperTest.Core.UseCases.Account.GetAccount;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ClearBank.DeveloperTest.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> logger;
        private readonly IMediator mediator;

        public AccountController(ILogger<AccountController> logger, IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }

        [HttpGet("{accountNumber}")]
        public async Task<Core.Models.Account> GetAccount([FromRoute] string accountNumber)
        {
            var request = new GetAccountQuery(accountNumber);
            var response = await mediator.Send(request);
            return response;
        }
    }
}
