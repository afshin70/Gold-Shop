using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class FavoritProductConfig : IEntityTypeConfiguration<FavoritProduct>
    {
        public void Configure(EntityTypeBuilder<FavoritProduct> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Customer).WithMany(x => x.FavoritProducts).HasForeignKey(x=>x.CustomerId);
            builder.HasOne(x => x.Product).WithMany(x => x.FavoritProducts).HasForeignKey(x=>x.ProductId);
        }
    }
}
