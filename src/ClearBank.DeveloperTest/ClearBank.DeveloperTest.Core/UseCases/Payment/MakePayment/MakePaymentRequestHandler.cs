using ClearBank.DeveloperTest.Core.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ClearBank.DeveloperTest.Core.UseCases.Payment.MakePayment
{
    public class MakePaymentRequestHandler : IRequestHandler<MakePaymentRequest, MakePaymentResult>
    {
        private readonly IPaymentService paymentService;

        public MakePaymentRequestHandler(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        public async Task<MakePaymentResult> Handle(MakePaymentRequest request, CancellationToken cancellationToken)
        {
            // Payment transaction flows from the debtor to the creditor
            var result = await paymentService.MakePayment(request);
            return result;
        }
    }
}
