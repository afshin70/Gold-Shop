using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(100);

            builder.HasOne(x => x.RegistrarUser).WithMany(x => x.Products).HasForeignKey(x => x.RegistrarUserId);
            builder.HasOne(x => x.Gallery).WithMany(x => x.Products).HasForeignKey(x => x.GalleryId);
        }
    }
}
