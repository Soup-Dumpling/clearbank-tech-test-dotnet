using Alba;
using Microsoft.Extensions.DependencyInjection;
using ClearBank.DeveloperTest.Infrastructure;
using System;
using System.Linq;

namespace ClearBank.DeveloperTest.API.IntegrationTests.Extentions
{
    public static class AlbaHostExtention
    {
        public static void WithEmptyDatabase(this IAlbaHost host, Action<ClearBankContext> action)
        {
            var scopeFactory = host.Server.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetService<ClearBankContext>();
            context.RemoveRange(context.Accounts.ToList());
            context.SaveChanges();
            action(context);
        }
    }
}
