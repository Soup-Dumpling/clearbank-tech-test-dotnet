using ClearBank.DeveloperTest.API.Models.Payment;
using ClearBank.DeveloperTest.Core.UseCases.Payment.MakePayment;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ClearBank.DeveloperTest.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> logger;
        private readonly IMediator mediator;

        public PaymentController(ILogger<PaymentController> logger, IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }

        [HttpPut]
        public async Task<MakePaymentResult> MakePayment([FromBody] MakePaymentRequestPayload model)
        {
            var request = new MakePaymentRequest(model.CreditorAccountNumber, model.DebtorAccountNumber, model.Amount, model.PaymentDate, model.PaymentScheme);
            var response = await mediator.Send(request);
            return response;
        }
    }
}
