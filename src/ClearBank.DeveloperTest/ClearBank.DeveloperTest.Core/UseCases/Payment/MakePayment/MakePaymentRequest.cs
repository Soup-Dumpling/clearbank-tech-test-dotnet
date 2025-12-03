using ClearBank.DeveloperTest.Core.Enums;
using MediatR;
using System;

namespace ClearBank.DeveloperTest.Core.UseCases.Payment.MakePayment
{
    public class MakePaymentRequest : IRequest<MakePaymentResult>
    {
        public string CreditorAccountNumber { get; set; }
        public string DebtorAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentScheme PaymentScheme { get; set; }

        public MakePaymentRequest(string creditorAccountNumber, string debtorAccountNumber, decimal amount, DateTime paymentDate, PaymentScheme paymentScheme) 
        {
            CreditorAccountNumber = creditorAccountNumber;
            DebtorAccountNumber = debtorAccountNumber;
            Amount = amount;
            PaymentDate = paymentDate;
            PaymentScheme = paymentScheme;
        }
    }
}
