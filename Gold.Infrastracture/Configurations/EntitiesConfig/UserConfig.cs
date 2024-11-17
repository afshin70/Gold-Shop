using Gold.Domain.Entities;
using Gold.Domain.Entities.AuthEntities;
using Gold.SharedKernel.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FullName).HasMaxLength(100).HasDefaultValue<string>(string.Empty);

            builder.Property(x => x.Email).HasMaxLength(100).HasColumnType("varchar");

            builder.Property(x => x.Mobile).HasMaxLength(11).HasColumnType("varchar");

            builder.Property(x => x.PasswordHash).HasMaxLength(100).IsRequired(true);

            builder.Property(x => x.PasswordSalt).HasMaxLength(100).IsRequired(true);

            builder.Property(x => x.UserName).HasMaxLength(50).IsRequired(true);

            builder.Property(x => x.WrongPasswordCount).HasDefaultValue(0);

            builder.HasOne(x => x.Customer).WithOne(x => x.User).HasForeignKey<Customer>(f => f.UserId);

            builder.HasOne(x => x.Seller).WithOne(x => x.User).HasForeignKey<Seller>(f => f.UserId);

            builder.HasOne(x => x.Manager).WithOne(x => x.User).HasForeignKey<Manager>(f => f.UserId);

            //builder.HasMany(x => x.GoldPrices).WithOne(x => x.User).HasForeignKey(x => x.UserId);
            //seed data 
            //user=> admin(username: pendarAdmin | password: 123456)
            //user=> admin(username: siteAdmin | password: 123456)
            string saltKey1 = Guid.NewGuid().ToString();
            string saltKey2 = Guid.NewGuid().ToString();

            var adminUsers = new List<User>
            {
                new User
                {
                    Id = 100000,
                    FullName="آراپندار",
                    IsActive=true,
                    Mobile=string.Empty,

                    PasswordHash=Encryptor.Encrypt("123456",saltKey1),
                    PasswordSalt=saltKey1,
                    RegisterDate=DateTime.Now,
                    SecurityStamp=Guid.NewGuid(),
                    UserName="pendarAdmin",
                    UserType=SharedKernel.Enums.UserType.Admin,
                },
                 new User
                {
                    Id = 100001,
                    FullName="مدیر سیستم",
                    IsActive=true,
                    Mobile=string.Empty,
                    PasswordHash=Encryptor.Encrypt("123456",saltKey2),
                    PasswordSalt=saltKey2,
                    RegisterDate=DateTime.Now,
                    SecurityStamp=Guid.NewGuid(),
                    UserName="siteAdmin",
                    UserType=SharedKernel.Enums.UserType.Admin,
                },
            };

            builder.HasData(adminUsers);


        }
    }
}
