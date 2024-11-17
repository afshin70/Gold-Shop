using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class FAQConfig : IEntityTypeConfiguration<FAQ>
    {
        public void Configure(EntityTypeBuilder<FAQ> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Question)
               //.HasMaxLength(100)
               .IsRequired();
            builder.Property(x => x.Answer)
               //.HasMaxLength(100)
               .IsRequired();
            builder.HasOne(x => x.Category).WithMany(x => x.FAQs).HasForeignKey(x => x.CategoryId);

        }
    }
}
