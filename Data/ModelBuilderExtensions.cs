using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using SaveUp.Models.Enum;
using SaveUp.Models.Transactions;

namespace SaveUp.Data
{
    public static class ModelBuilderExtensions
    {

        public static void ConfigureEntinties(this ModelBuilder builder)
        {
            builder.Entity<FeeRule>().HasIndex(f => f.Name).IsUnique();
            builder.Entity<Currency>().HasIndex(c => c.Name).IsUnique();
            builder.Entity<TransactionType>().HasIndex(t => t.Name).IsUnique();
            builder.Entity<ClientSegment>().HasIndex(c => c.Name).IsUnique();

            builder.Entity<FeeCalculationHistory>()
            .HasMany(f => f.FeeRules)
            .WithMany(f => f.FeeCalculationHistories)
            .UsingEntity(j => j.ToTable("FeeRuleHistories"));
        }

        public static Task SeedData(this ModelBuilder modelBuilder)
        {
            // Seed data logic can be implemented here

            FeeRule[] FeeRules =
            [
                new FeeRule
                {
                    Id = 1,
                    Name = "POS under 100€ – Fixed",
                    ConditionExpression = "Type == 'POS' && Amount <= 100",
                    CalculationExpression = "0.20"
                },
                new FeeRule
                {
                    Id = 2,
                    Name = "POS over 100€ – Percentage",
                    ConditionExpression = "Type == 'POS' && Amount > 100",
                    CalculationExpression = "Amount * 0.002" // 0.2%
                },
                new FeeRule
                {
                    Id = 3,
                    Name = "E-Commerce – Percent + Fixed",
                    ConditionExpression = "Type == 'e-commerce'",
                    CalculationExpression = "Amount * 0.018 + 0.15",
                    MaxFee = (double?)120m
                },
                new FeeRule
                {
                    Id = 4,
                    Name = "High Credit Score – Discount",
                    ConditionExpression = "CreditScore > 400",
                    CalculationExpression = "0", // dummy fee; discount is applied to the total
                    DiscountPercent = (double?)1m
                },
            ];
            modelBuilder.Entity<FeeRule>().HasData(FeeRules);

            Currency[] Currencies =
            [
                new Currency
                {
                    Id = 1,
                    Name = "EUR",
                },
                new Currency
                {
                    Id = 2,
                    Name = "MKD",
                },
                new Currency
                {
                    Id = 3,
                    Name = "USD",
                },

            ];
            modelBuilder.Entity<Currency>().HasData(Currencies);

            ClientSegment[] ClientSegments =
            [
                new ClientSegment
                {
                    Id = 1,
                    Name = "Regular",
                },
                new ClientSegment
                {
                    Id = 2,
                    Name = "Trusted",
                },
                new ClientSegment
                {
                    Id = 3,
                    Name = "Premium",
                },
            ];
            modelBuilder.Entity<ClientSegment>().HasData(ClientSegments);


            Client[] Clients =
            [
                new Client
                {
                    Id = 1,
                    CreditScore = 400,
                    RiskLevel = RiskLevel.Low,
                    ClientSegmentId = ClientSegments[0].Id                },
                new Client
                {
                    Id = 2,
                    CreditScore = 400,
                    RiskLevel = RiskLevel.Low,
                    ClientSegmentId = ClientSegments[0].Id
                },
            ];
            modelBuilder.Entity<Client>().HasData(Clients);

            TransactionType[] TransactionTypes =
            [
                new TransactionType
                {
                    Id = 1,
                    Name = "POS",
                },
                new TransactionType
                {
                    Id = 2,
                    Name = "e-commerce",
                },
                new TransactionType
                {
                    Id = 3,
                    Name = "ATM",
                },
            ];
            modelBuilder.Entity<TransactionType>().HasData(TransactionTypes);


            return Task.CompletedTask;
        }

    }
}
