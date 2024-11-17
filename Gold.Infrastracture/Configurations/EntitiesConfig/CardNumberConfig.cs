using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
	public class CardNumberConfig : IEntityTypeConfiguration<BankCardNo>
    {
        public void Configure(EntityTypeBuilder<BankCardNo> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Number).HasMaxLength(20).HasColumnType("varchar");
            builder.Property(x => x.Owner).HasMaxLength(100);

        }
    } 
}
