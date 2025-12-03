using ClearBank.DeveloperTest.Core.Enums;
using System;

namespace ClearBank.DeveloperTest.API.Models.Payment
{
    public class MakePaymentRequestPayload
    {
        public string CreditorAccountNumber { get; set; }
        public string DebtorAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentScheme PaymentScheme { get; set; }
    }
}
