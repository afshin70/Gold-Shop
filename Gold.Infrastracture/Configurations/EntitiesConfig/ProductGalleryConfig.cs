using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class ProductGalleryConfig : IEntityTypeConfiguration<ProductGallery>
    {
        public void Configure(EntityTypeBuilder<ProductGallery> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FileName).HasMaxLength(100);
            builder.HasOne(x => x.Product).WithMany(x => x.ProductGalleries).HasForeignKey(x => x.ProductId);
        }
    }
}
