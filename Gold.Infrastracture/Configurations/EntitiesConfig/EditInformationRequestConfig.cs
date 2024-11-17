using Gold.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gold.Infrastracture.Configurations.EntitiesConfig
{
    public class EditInformationRequestConfig : IEntityTypeConfiguration<EditInformationRequest>
    {
        public void Configure(EntityTypeBuilder<EditInformationRequest> builder)
        {
            builder.Property(x => x.ImageName).HasMaxLength(50).HasColumnType("varchar");


        }
    }
}
