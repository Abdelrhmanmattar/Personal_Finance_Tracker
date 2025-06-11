using Core.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.MODELS.CONFIG
{
    public class BudgetUserConfig : IEntityTypeConfiguration<BudgetUser>
    {
        public void Configure(EntityTypeBuilder<BudgetUser> builder)
        {
            builder.ToTable("BudgetUser");

            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(b=>b.Role)
                .HasColumnType("NVARCHAR")
                .HasMaxLength(7).IsRequired();

            builder.Property(b => b.BudgetId).IsRequired();
            builder.Property(b => b.UserId).IsRequired();

            builder.HasOne(b => b.User)
                .WithMany(u => u.BudgetUsers)
                .HasForeignKey(b => b.UserId)
                .HasPrincipalKey(u => u.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Budget)
                .WithMany(u => u.BudgetUsers)
                .HasForeignKey(b => b.BudgetId)
                .HasPrincipalKey(u => u.Id)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }


}
