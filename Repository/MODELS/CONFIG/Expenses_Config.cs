using Core.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.MODELS.CONFIG
{
    public class Expenses_Config : IEntityTypeConfiguration<Expenses>
    {
        public void Configure(EntityTypeBuilder<Expenses> builder)
        {
            builder.ToTable("Expenses");

            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(i => i.Amount).HasPrecision(10,2).IsRequired();
            builder.Property(b => b.User_Id).IsRequired();
            builder.Property(b => b.BudgetId).IsRequired();

            builder.Property(b => b.Date_Withdraw)
                .HasColumnType("DateTime")
                .IsRequired();



            builder.HasOne(e => e.User_App)
                .WithMany(u => u.Expenses)
                .HasForeignKey(e => e.User_Id)
                .HasPrincipalKey(u => u.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Budget)
                .WithMany(b => b.Expenses)
                .HasForeignKey(e => e.BudgetId)
                .HasPrincipalKey(b => b.Id);
        }
    }
}
