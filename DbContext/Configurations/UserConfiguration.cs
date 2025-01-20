using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.API.Models;

namespace TaskManagement.API.DbContext.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.FullName)
                .HasMaxLength(256)
                .IsRequired();
            builder.Property(x => x.CreatedAt)
                .IsRequired();
            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            builder.Property(x => x.Email)
                .IsRequired();
            builder.HasIndex(x => x.Email)
                .IsUnique();


        }
    }
}
