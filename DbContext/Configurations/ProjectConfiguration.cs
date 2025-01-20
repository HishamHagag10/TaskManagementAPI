using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.API.Models;

namespace TaskManagement.API.DbContext.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(1000)
                .IsRequired();

            builder.HasOne(x => x.ProjectManager)
                .WithMany(x => x.ManageProjects)
                .HasForeignKey(x => x.ProjectManagerEmail)
                .HasPrincipalKey(x => x.Email)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(x => x.WorkingUsers)
                .WithMany(x => x.ProjectsWorkingAt)
                .UsingEntity<UserProject>(
                l => l.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserEmail)
                .HasPrincipalKey(x => x.Email),

                r => r.HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectId),

                j => j.HasKey(x => new { x.UserEmail, x.ProjectId })
                );
        }
    }
}
