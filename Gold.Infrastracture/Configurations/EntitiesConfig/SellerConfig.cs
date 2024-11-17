using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class SellerConfig : IEntityTypeConfiguration<Seller>
    {
        public void Configure(EntityTypeBuilder<Seller> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ImageName).HasMaxLength(50);

            builder.HasOne(x => x.Gallery).WithMany(x=>x.Sellers).HasForeignKey(f => f.GalleryId).OnDelete(DeleteBehavior.NoAction);
        }
    } 
}
