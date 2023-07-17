using Microsoft.EntityFrameworkCore;
using PFM.DataAccess.Configurations;
using PFM.DataAccess.Entities;
using System.Reflection;

namespace PFM.DataAccess.DbContextOption
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<TransactionSplit> TransactionsSplits { get; set; }
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionSplitConfiguration());

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
