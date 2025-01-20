using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.API.Models;

namespace TaskManagement.API.DbContext.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.Property(x => x.Message)
                .HasMaxLength(256)
                .IsRequired();
            builder.Property(x => x.Type)
                .HasMaxLength(256)
                .IsRequired();
            builder.HasOne(x => x.ToUser)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.ToUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
