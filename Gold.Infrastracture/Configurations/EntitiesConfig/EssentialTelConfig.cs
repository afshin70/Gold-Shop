using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class EssentialTelConfig : IEntityTypeConfiguration<EssentialTel>
    {
        public void Configure(EntityTypeBuilder<EssentialTel> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.RelationShip).HasMaxLength(100);
            builder.Property(x => x.Tel).HasMaxLength(50).HasColumnType("varchar");

        }
    }  
}
