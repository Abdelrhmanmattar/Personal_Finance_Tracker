using Core.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.MODELS.CONFIG
{
    public class UserApp_Config : IEntityTypeConfiguration<Users_App>
    {
        public void Configure(EntityTypeBuilder<Users_App> builder)
        {
            //public DateTime Created_At { get; set; }

            builder.Property(u => u.Created_At)
                .HasColumnType("DateTime")
                .IsRequired();
        }
    }
}
