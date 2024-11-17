using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class PasswordRecoveryConfig : IEntityTypeConfiguration<SendSmsTemp>
    {
        public void Configure(EntityTypeBuilder<SendSmsTemp> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x=>x.Code).HasMaxLength(10).HasColumnType("varchar").IsRequired(false);
            builder.Property(x => x.Token);
            builder.Property(x => x.ExpireDate);
            builder.Property(x => x.SendDate);
            builder.Property(x => x.UserName);
            builder.Property(x => x.Mobile);
        }
    }
}
