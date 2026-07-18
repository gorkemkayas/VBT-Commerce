using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Domain.Entities;

namespace Notification.Infrastructure.Persistence.Configurations;

public class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
{
    public void Configure(EntityTypeBuilder<NotificationLog> builder)
    {
        builder.ToTable("NotificationLogs");
        builder.HasKey(l => l.Id);

        builder.HasIndex(l => l.ReferenceId);

        builder.Property(l => l.NotificationType).HasMaxLength(100).IsRequired();
        builder.Property(l => l.RecipientEmail).HasMaxLength(320).IsRequired();
        builder.Property(l => l.Subject).HasMaxLength(300).IsRequired();
        builder.Property(l => l.Body).IsRequired();
        builder.Property(l => l.ErrorMessage).HasMaxLength(2000);
    }
}
