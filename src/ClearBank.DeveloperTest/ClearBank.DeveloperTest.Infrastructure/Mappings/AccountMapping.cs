using ClearBank.DeveloperTest.Core.Enums;
using ClearBank.DeveloperTest.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ClearBank.DeveloperTest.Infrastructure.Mappings
{
    public class AccountMapping : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder) 
        {
            builder.HasKey(x => x.AccountNumber);
            builder.Property(x => x.Balance).HasColumnType("decimal(18, 2)").HasDefaultValue(0.00M);
            builder.Property(x => x.Status).HasConversion(new EnumToStringConverter<AccountStatus>());
            builder.Property(x => x.AllowedPaymentSchemes).HasConversion(new EnumToStringConverter<AllowedPaymentSchemes>());
            builder.HasData(
                new Account() { AccountNumber = "94202716", Balance = 63254.88M, Status = AccountStatus.Live, AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments },
                new Account() { AccountNumber = "45302248", Balance = 21098.76M, Status = AccountStatus.Live, AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs },
                new Account() { AccountNumber = "09773439", Balance = 3109.87M, Status = AccountStatus.Live, AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps },
                new Account() { AccountNumber = "57047920", Balance = 2678.90M, Status = AccountStatus.Live, AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs },
                new Account() { AccountNumber = "23992701", Balance = 89012.34M, Status = AccountStatus.Live, AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps },
                new Account() { AccountNumber = "31022756", Balance = 56789.01M, Status = AccountStatus.Live, AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps },
                new Account() { AccountNumber = "55694617", Balance = 43210.98M, Status = AccountStatus.Live, AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps },
                new Account() { AccountNumber = "08845741", Balance = 32987.65M, Status = AccountStatus.Live, AllowedPaymentSchemes = 0 },
                new Account() { AccountNumber = "65015197", Balance = 8432.10M, Status = AccountStatus.Disabled, AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs },
                new Account() { AccountNumber = "99636544", Balance = 58327.56M, Status = AccountStatus.Disabled, AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps },
                new Account() { AccountNumber = "14653248", Balance = -250.50M, Status = AccountStatus.InboundPaymentsOnly, AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments },
                new Account() { AccountNumber = "36946349", Balance = -750.25M, Status = AccountStatus.InboundPaymentsOnly, AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs },
                new Account() { AccountNumber = "74000307", Balance = -1500.75M, Status = AccountStatus.InboundPaymentsOnly, AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps });
        }
    }
}
