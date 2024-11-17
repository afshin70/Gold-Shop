using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class SettingConfig : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder
                .HasKey(x => x.Id);
            builder
                .Property(x => x.Value)
                .HasMaxLength(2000)
                .IsRequired(true);
            builder
                .Property(x => x.Type)
                .IsUnicode(true)
                .IsRequired(true);
        }
    }
}
