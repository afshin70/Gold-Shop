using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class DocumentConfig : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x=>x.DocumentDate).HasColumnType("date");
            builder.Property(x=>x.SettleDate).HasColumnType("date");
            builder.Property(x=>x.DeliveryDate).HasColumnType("date");
            builder.HasOne(x => x.Customer).WithMany(x=>x.Documents).HasForeignKey(f => f.CustomerId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Gallery).WithMany(x=>x.Documents).HasForeignKey(f => f.GalleryId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Seller).WithMany(x=>x.Documents).HasForeignKey(f => f.SellerId).OnDelete(DeleteBehavior.NoAction);

        }
    }
}
