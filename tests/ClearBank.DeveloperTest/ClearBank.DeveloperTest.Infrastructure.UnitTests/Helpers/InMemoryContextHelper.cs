using Microsoft.EntityFrameworkCore;
using System;

namespace ClearBank.DeveloperTest.Infrastructure.UnitTests.Helpers
{
    public static class InMemoryContextHelper
    {
        public static ClearBankContext GetContext()
        {
            var builder = new DbContextOptionsBuilder<ClearBankContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            return new ClearBankContext(builder.Options);
        }
    }
}
