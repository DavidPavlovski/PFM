using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFM.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
