using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class AdminMenuConfig : IEntityTypeConfiguration<AdminMenu>
    {
        public void Configure(EntityTypeBuilder<AdminMenu> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.Title).HasMaxLength(50);
            builder.Property(x => x.IconName).HasMaxLength(30).HasColumnType("varchar");
            builder.Property(x => x.ControllerName).HasMaxLength(30).HasColumnType("varchar");
            builder.Property(x => x.ActionName).HasMaxLength(30).HasColumnType("varchar");

            builder.HasOne(x => x.AdminMenuGroup).WithMany(x => x.AdminMenus)
                .HasForeignKey(f => f.AdminMenuGroupID).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.PermissionAccesses).WithOne(x => x.AdminMenu)
                .HasForeignKey(x => x.AdminMenuId).OnDelete(DeleteBehavior.NoAction);


            //var list = new List<AdminMenu> { };
            //builder.HasData(list);

        }
    }
}
