using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class PermissionAccessConfig : IEntityTypeConfiguration<PermissionAccess>
    {
        public void Configure(EntityTypeBuilder<PermissionAccess> builder)
        {
            builder.HasKey(x => x.Id);

           

            
        }
    }
}
