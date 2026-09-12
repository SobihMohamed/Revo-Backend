using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Infrastructure.Database.Configuration
{
    public class PortfolioItemConfiguration : IEntityTypeConfiguration<PortfolioItem>
    {
        public void Configure(EntityTypeBuilder<PortfolioItem> builder)
        {
            builder.ToTable("PortfolioItems");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.CaptionAr).IsRequired().HasMaxLength(250);
            builder.Property(p => p.CaptionEn).IsRequired().HasMaxLength(250);
            builder.Property(p => p.OrderIndex).HasDefaultValue(0);

            builder.HasMany(p => p.MediaItems)
                   .WithOne(m => m.PortfolioItem)
                   .HasForeignKey(m => m.PortfolioItemId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}