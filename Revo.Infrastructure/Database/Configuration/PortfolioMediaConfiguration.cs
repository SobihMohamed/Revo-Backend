using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Infrastructure.Database.Configuration
{
    public class PortfolioMediaConfiguration : IEntityTypeConfiguration<PortfolioMedia>
    {
        public void Configure(EntityTypeBuilder<PortfolioMedia> builder)
        {
            builder.ToTable("PortfolioMedia");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.MediaUrl).IsRequired().HasMaxLength(500);

            builder.Property(m => m.CoverImageUrl).HasMaxLength(500).IsRequired(false);

            builder.Property(m => m.Type).IsRequired(); 
            builder.Property(m => m.OrderIndex).HasDefaultValue(0);
        }
    }
}
