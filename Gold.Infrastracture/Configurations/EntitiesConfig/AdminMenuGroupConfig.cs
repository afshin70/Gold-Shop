using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class AdminMenuGroupConfig : IEntityTypeConfiguration<AdminMenuGroup>
    {
        public void Configure(EntityTypeBuilder<AdminMenuGroup> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.Title).HasMaxLength(50);
            builder.Property(x => x.IconName).HasMaxLength(30).HasColumnType("varchar");

        }
    }
}
