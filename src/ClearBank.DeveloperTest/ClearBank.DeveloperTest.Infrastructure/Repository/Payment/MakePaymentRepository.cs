using ClearBank.DeveloperTest.Core.UseCases.Payment.MakePayment;
using System.Threading.Tasks;

namespace ClearBank.DeveloperTest.Infrastructure.Repository.Payment
{
    public class MakePaymentRepository : IMakePaymentRepository
    {
        private readonly ClearBankContext context;

        public MakePaymentRepository(ClearBankContext context) 
        {
            this.context = context;
        }

        public async Task UpdateAccounts()
        {
            await context.SaveChangesAsync();
        }
    }
}
