using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class CustomerConfig : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.NationalCode).HasMaxLength(50).HasColumnType("varchar");
            builder.Property(x => x.FatherName).HasMaxLength(100);
            builder.Property(x => x.JobTitle).HasMaxLength(100);
            builder.Property(x => x.PostalCode).HasMaxLength(10).HasColumnType("varchar");
            builder.Property(x => x.Address).HasMaxLength(500);

            //builder.Property(x => x.BirthDateDay).HasMaxLength(2).HasColumnType("varchar");
            //builder.Property(x => x.BirthDateMonth).HasMaxLength(2).HasColumnType("varchar");
            //builder.Property(x => x.BirthDateYear).HasMaxLength(4).HasColumnType("varchar");

            builder.HasOne(x => x.City).WithMany(x => x.Customers).HasForeignKey(f => f.CityId);
            builder.HasMany(x => x.EssentialTels).WithOne(x => x.Customer).HasForeignKey(x => x.CustomerId);
            builder.HasMany(x => x.BankCardNo).WithOne(x => x.Customer).HasForeignKey(x => x.CustomerId);
            builder.HasMany(x => x.ProfileImages).WithOne(x => x.Customer).HasForeignKey(x => x.CustomerId);
            builder.HasMany(x => x.EditInformationRequests).WithOne(x => x.Customer).HasForeignKey(x => x.CustomerId);


        }
    }
}
