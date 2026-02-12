using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using SaveUp.Models.Transactions;

namespace SaveUp.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions options) :
            base(options)
        {
        }

        public DbSet<FeeRule> FeeRules { get; set; }
        public DbSet<FeeCalculationHistory> FeeCalculationHistories { get; set; }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<Currency> Currencies { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<ClientSegment> ClientSegments { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ConfigureEntinties();
            modelBuilder.SeedData().Wait();
        }

    }
}
