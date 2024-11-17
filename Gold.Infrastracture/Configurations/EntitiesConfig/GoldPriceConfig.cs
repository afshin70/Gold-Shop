using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class GoldPriceConfig : IEntityTypeConfiguration<GoldPrice>
    {
        public void Configure(EntityTypeBuilder<GoldPrice> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x=>x.User).WithMany(x=>x.GoldPrices).HasForeignKey(x=>x.UserId);
        }
    }  
}
