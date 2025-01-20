using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.API.Models;

namespace TaskManagement.API.DbContext.Configurations
{
    public class TaskConfiguration : IEntityTypeConfiguration<Models.Task>
    {
        public void Configure(EntityTypeBuilder<Models.Task> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Description)
                .HasMaxLength(1000).IsRequired();

            builder.Property(p => p.Title)
                .HasMaxLength(100).IsRequired();

            builder.HasOne(x => x.Project)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.ProjectId)
                ;

            builder.HasOne(x => x.AssignedUser)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.AssignedUserEmail)
                .HasPrincipalKey(x => x.Email)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(x => x.Comments)
                .WithOne(x => x.Task)
                .HasForeignKey(x => x.TaskId);

            builder.HasMany(x => x.Tags)
                .WithMany(x => x.Tasks)
                .UsingEntity<TaskTag>
                (
                l => l.HasOne(x => x.Tag).WithMany().HasForeignKey(x => x.TagId),
                r => r.HasOne(x => x.Task).WithMany().HasForeignKey(x => x.TaskId)
                ).HasKey(x => new { x.TaskId, x.TagId })
                ;

        }
    }
}
