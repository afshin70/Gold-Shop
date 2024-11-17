using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class FAQCategoryConfig : IEntityTypeConfiguration<FAQCategory>
    {
        public void Configure(EntityTypeBuilder<FAQCategory> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
