using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Infrastructure.Database.Configuration
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.ToTable("Services");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.NameAr).IsRequired().HasMaxLength(150);
            builder.Property(s => s.NameEn).IsRequired().HasMaxLength(150);

            builder.Property(s => s.DescriptionAr).IsRequired().HasMaxLength(1000);
            builder.Property(s => s.DescriptionEn).IsRequired().HasMaxLength(1000);

            builder.Property(s => s.ImageUrl).IsRequired(false).HasMaxLength(500);
            builder.Property(s => s.OrderIndex).HasDefaultValue(0);
        }
    }
}
