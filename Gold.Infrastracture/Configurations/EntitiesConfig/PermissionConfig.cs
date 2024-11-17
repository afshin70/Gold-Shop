using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class PermissionConfig : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).HasMaxLength(100);

            builder.HasMany(x => x.PermissionAccesses).WithOne(x => x.Permission)
               .HasForeignKey(f => f.PermissionId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
