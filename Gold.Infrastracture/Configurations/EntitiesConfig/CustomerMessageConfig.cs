using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class CustomerMessageConfig : IEntityTypeConfiguration<CustomerMessage>
    {
        public void Configure(EntityTypeBuilder<CustomerMessage> builder)
        {
            builder.Property(x => x.Title).HasMaxLength(200);

            builder.HasOne(x => x.Customer).WithMany(x => x.CustomerMessages).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Document).WithMany(x => x.CustomerMessages).HasForeignKey(x => x.DocumentId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Installment).WithMany(x => x.CustomerMessages).HasForeignKey(x => x.installmentId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
