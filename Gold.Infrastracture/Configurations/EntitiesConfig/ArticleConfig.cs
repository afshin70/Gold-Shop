using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class ArticleConfig : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Title)
                .HasMaxLength(250)
                .IsRequired();
            builder
               .Property(x => x.ImageFileName)
               .HasMaxLength(100)
               .IsRequired(false);

            builder
               .Property(x => x.VideoFileName)
               .HasMaxLength(100)
               .IsRequired(false);

            builder
               .Property(x => x.Description)
               .IsRequired(false);
        }
    }
}
