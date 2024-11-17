using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class ContactUsConfig : IEntityTypeConfiguration<ContactUs>
    {
        public void Configure(EntityTypeBuilder<ContactUs> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Message).HasMaxLength(1000).IsRequired(true);
            builder.Property(x=>x.FullName).HasMaxLength(100).IsRequired(true);
            builder.Property(x=>x.Phone).HasMaxLength(11).IsRequired(false);
        }
    } 
}
