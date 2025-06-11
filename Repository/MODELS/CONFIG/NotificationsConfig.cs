    using Core.entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    namespace Repository.MODELS.CONFIG
    {
        public class NotificationsConfig : IEntityTypeConfiguration<Notifications>
        {
            public void Configure(EntityTypeBuilder<Notifications> builder)
            {

                    builder.ToTable("Notifications");

                    builder.HasKey(n => n.Id);
                    builder.Property(b => b.Id)
                        .ValueGeneratedOnAdd()
                        .IsRequired();

                    builder.Property(n => n.Title)
                        .HasColumnType("NVARCHAR")
                        .HasMaxLength(50)
                        .IsRequired();

                    builder.Property(n => n.Message)
                        .HasColumnType("NVARCHAR")
                        .HasMaxLength(500)
                        .IsRequired(false);

                    builder.Property(n => n.Type)
                        .HasColumnType("NVARCHAR")
                        .HasMaxLength(50)
                        .IsRequired(false);

                    builder.Property(n => n.CreatedAt)
                        .IsRequired();

                    builder.Property(n=>n.IsRead).IsRequired();

                    builder.HasOne(n=>n.User_App)
                        .WithMany(u=> u.Notifications)
                        .HasForeignKey(n=>n.UserId)
                        .HasPrincipalKey(u=>u.Id)
                        .OnDelete(DeleteBehavior.Restrict);




            }
        }

    }
