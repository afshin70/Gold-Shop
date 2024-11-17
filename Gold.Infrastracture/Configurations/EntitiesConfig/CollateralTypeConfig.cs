using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class CollateralTypeConfig : IEntityTypeConfiguration<CollateralType>
    {
        public void Configure(EntityTypeBuilder<CollateralType> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).HasMaxLength(50);

            var list = new List<CollateralType>();
            list.Add(new CollateralType { Title = "سکه", Id = 1 });
            list.Add(new CollateralType { Title = "گرم", Id = 2 });
            list.Add(new CollateralType { Title = "سند", Id = 3 });
            list.Add(new CollateralType { Title = "چک", Id = 4 });
            list.Add(new CollateralType { Title = "گالری", Id = 5 });
            builder.HasData(list);
        }
    }
}
