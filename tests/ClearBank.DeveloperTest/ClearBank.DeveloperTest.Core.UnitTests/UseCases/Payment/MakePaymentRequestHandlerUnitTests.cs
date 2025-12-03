using ClearBank.DeveloperTest.Core.Enums;
using ClearBank.DeveloperTest.Core.Services;
using ClearBank.DeveloperTest.Core.UseCases.Payment.MakePayment;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClearBank.DeveloperTest.Core.UnitTests.UseCases.Payment
{
    public class MakePaymentRequestHandlerUnitTests
    {
        private readonly MakePaymentRequestHandler makePaymentRequestHandler;
        private readonly IPaymentService mockPaymentService = Substitute.For<IPaymentService>();

        public MakePaymentRequestHandlerUnitTests() 
        {
            makePaymentRequestHandler = new MakePaymentRequestHandler(mockPaymentService);
        }

        [Fact]
        public async Task Handle_CallsPaymentServiceAndReturnsResult()
        {
            // Arrange
            var request = new MakePaymentRequest("90580035", "21912910", 250.00M, DateTime.UtcNow, PaymentScheme.FasterPayments);
            var expectedResult = new MakePaymentResult() { Success = true };
            mockPaymentService.MakePayment(Arg.Any<MakePaymentRequest>()).Returns(Task.FromResult(expectedResult));

            // Act
            var result = await makePaymentRequestHandler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(expectedResult, result);
            Assert.IsType<MakePaymentResult>(result);
            await mockPaymentService.Received().MakePayment(request);
        }
    }
}
