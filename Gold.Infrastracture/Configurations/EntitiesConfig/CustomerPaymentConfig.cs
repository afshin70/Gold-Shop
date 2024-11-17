using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class CustomerPaymentConfig : IEntityTypeConfiguration<CustomerPayment>
    {
        public void Configure(EntityTypeBuilder<CustomerPayment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ImageName).HasMaxLength(100).HasColumnType("varchar");
            builder.HasOne(x => x.Document).WithMany(x=>x.CustomerPayments).HasForeignKey(f => f.DocumentId).OnDelete(DeleteBehavior.NoAction);

        }
    }
}
