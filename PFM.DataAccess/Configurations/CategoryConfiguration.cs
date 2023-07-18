using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFM.DataAccess.Entities;

namespace PFM.DataAccess.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.Code);
            builder.HasMany(x => x.Transactions);
            builder.Property(x => x.ParentCode).IsRequired(false);
        }
    }
}
