using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Infrastructure.Database.Configuration
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");
            builder.HasKey(n => n.Id);

            builder.Property(n => n.TitleAr).IsRequired().HasMaxLength(200);
            builder.Property(n => n.TitleEn).IsRequired().HasMaxLength(200);
            builder.Property(n => n.MessageAr).IsRequired().HasMaxLength(500);
            builder.Property(n => n.MessageEn).IsRequired().HasMaxLength(500);
            builder.Property(n => n.IsRead).HasDefaultValue(false);
            builder.Property(n => n.TargetUrl).HasMaxLength(500);

        }
    }
}
