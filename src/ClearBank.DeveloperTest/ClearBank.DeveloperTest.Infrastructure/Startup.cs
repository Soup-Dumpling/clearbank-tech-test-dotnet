using ClearBank.DeveloperTest.Core.UseCases.Account.GetAccount;
using ClearBank.DeveloperTest.Core.UseCases.Payment.MakePayment;
using ClearBank.DeveloperTest.Infrastructure.Repository.Account;
using ClearBank.DeveloperTest.Infrastructure.Repository.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ClearBank.DeveloperTest.Infrastructure
{
    public static class Startup
    {
        public static void AddInfrastructureBindings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ClearBankContext>(opt =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                opt.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                });
            });

            services.AddScoped<IGetAccountRepository, GetAccountRepository>();
            services.AddScoped<IMakePaymentRepository, MakePaymentRepository>();
        }
    }
}
