using Gold.Domain.Entities.LoggingEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.SystemActivityEntitiesConfig
{
    public class SystemActivityLogConfig : IEntityTypeConfiguration<SystemActivityLog>
    {
        public void Configure(EntityTypeBuilder<SystemActivityLog> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Message).HasMaxLength(2000);
            builder.Property(x => x.ExtraData).HasMaxLength(4000);
            builder.Property(x => x.Source).HasMaxLength(1500);

        }
    }
}
