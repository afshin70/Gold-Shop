using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class InstallmentConfig : IEntityTypeConfiguration<Installment>
    {
        public void Configure(EntityTypeBuilder<Installment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Date).HasColumnType("date");
            //builder.Property(x => x.PaymentDate).HasColumnType("date");
            builder.HasOne(x => x.Document).WithMany(x=>x.Installments).HasForeignKey(f => f.DocumentId).OnDelete(DeleteBehavior.NoAction);


        }
    }
}
