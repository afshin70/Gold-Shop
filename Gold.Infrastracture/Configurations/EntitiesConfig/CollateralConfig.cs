using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class CollateralConfig : IEntityTypeConfiguration<Collateral>
    {
        public void Configure(EntityTypeBuilder<Collateral> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Document).WithMany(x=>x.Collaterals).HasForeignKey(f => f.DocumentId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.CollateralType).WithMany(x=>x.Collaterals).HasForeignKey(f => f.CollateralTypeId).OnDelete(DeleteBehavior.NoAction);

        }
    }
}
