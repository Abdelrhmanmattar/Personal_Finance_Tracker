using Core.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.MODELS.CONFIG
{
    public class Category_Config : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Category");

            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();


            builder.Property(i => i.Name)
                .HasColumnType("NVARCHAR")
                .HasMaxLength(50).IsRequired();


            builder.Property(b => b.User_Id).IsRequired();
            
            builder.HasOne(b => b.User_App)
                .WithMany(u => u.Categories)
                .HasForeignKey(b => b.User_Id)
                .HasPrincipalKey(u=>u.Id)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
