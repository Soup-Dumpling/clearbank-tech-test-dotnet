using Alba;
using ClearBank.DeveloperTest.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ClearBank.DeveloperTest.API.IntegrationTests
{
    public class AppFixture : IDisposable, IAsyncLifetime
    {
        public IAlbaHost Host { get; private set; }
        private readonly InMemoryDatabaseRoot _dbRoot = new();
        public void Dispose()
        {
            Host?.Dispose();
        }

        public async Task InitializeAsync()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            Environment.SetEnvironmentVariable("DisableMigration", "true");

            Host = await Program
                .CreateHostBuilder(Array.Empty<string>())
                .ConfigureServices((config, services) =>
                {
                    var descriptors = services.Where(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<ClearBankContext>)).ToList();

                    foreach (var descriptor in descriptors)
                    {
                        services.Remove(descriptor);
                    }
                    services.AddDbContext<ClearBankContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting", _dbRoot);
                    });
                })
                .StartAlbaAsync();
        }

        public async Task DisposeAsync()
        {
            await Host.StopAsync();
        }
    }

    [CollectionDefinition("Integration")]
    public class AppFixtureCollection : ICollectionFixture<AppFixture>
    {

    }
}
