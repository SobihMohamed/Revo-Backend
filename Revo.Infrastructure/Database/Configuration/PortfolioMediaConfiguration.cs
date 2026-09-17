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

            builder.Property(x => x.MediaUrl).IsRequired().HasMaxLength(500);
            builder.Property(x => x.MediaPublicId).IsRequired().HasMaxLength(255);

            builder.Property(x => x.CoverImageUrl).HasMaxLength(500);
            builder.Property(x => x.CoverImagePublicId).HasMaxLength(255);

            builder.Property(x => x.OrderIndex).HasDefaultValue(0); builder.Property(m => m.Type).IsRequired(); 
        }
    }
}
