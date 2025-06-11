using Core.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.MODELS.CONFIG
{
    public class BudgetsConfig : IEntityTypeConfiguration<Budgets>
    {
        public void Configure(EntityTypeBuilder<Budgets> builder)
        {
            builder.ToTable("Budgets");

            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(b=>b.LimitAmount)
                .HasPrecision(10,2)
                .IsRequired();

            builder.Property(b => b.User_Id)
                .IsRequired();

            builder.Property(b => b.Cat_Id)
                .IsRequired();


            builder.HasOne(b=>b.User_App)
                .WithMany(u=>u.Budgets)
                .HasForeignKey(b=>b.User_Id)
                .HasPrincipalKey(u=>u.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b=>b.Category)
                .WithMany(c=>c.Budgets)
                .HasForeignKey(b=>b.Cat_Id)
                .HasPrincipalKey(c=>c.Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }


}
