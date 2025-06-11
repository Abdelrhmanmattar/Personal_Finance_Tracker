using Core.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.MODELS.CONFIG
{
    public class IncomeConfig : IEntityTypeConfiguration<Income>
    {
        public void Configure(EntityTypeBuilder<Income> builder)
        {
            builder.ToTable("Income");

            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(i => i.Amount).HasPrecision(10, 2).IsRequired();

            builder.Property(i => i.Source)
                .HasColumnType("NVARCHAR")
                .HasMaxLength(100).IsRequired();


            builder.Property(b => b.User_Id).IsRequired();

            builder.Property(b => b.Date_Deposite)
                .HasColumnType("DateTime")
                .IsRequired();

            builder.HasOne(b => b.User_App)
                .WithMany(u => u.Incomes)
                .HasForeignKey(b => b.User_Id)
                .HasPrincipalKey(u => u.Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
