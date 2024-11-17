using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class PaymentConfig : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Date).HasColumnType("date");
            builder.HasOne(x => x.Installment).WithMany(x=>x.Payments).HasForeignKey(f => f.InstallmentId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.CustomerPayment).WithMany(x=>x.Payments).HasForeignKey(f => f.CustomerPaymentId).OnDelete(DeleteBehavior.NoAction).IsRequired(false);


        }
    }
}
