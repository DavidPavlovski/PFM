using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFM.DataAccess.Entities;

namespace PFM.DataAccess.Configurations
{
    public class TransactionSplitConfiguration : IEntityTypeConfiguration<TransactionSplit>
    {
        public void Configure(EntityTypeBuilder<TransactionSplit> builder)
        {
            builder.HasKey(x => x.Id);
            builder
                .HasOne(x => x.Transaction)
                .WithMany(x => x.TransactionSplits)
                .HasForeignKey(x => x.TransactionId);
        }
    }
}
