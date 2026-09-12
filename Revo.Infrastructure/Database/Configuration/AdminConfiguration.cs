using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Infrastructure.Database.Configuration
{
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.ToTable("Admins");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.FullName)
                   .IsRequired()
                   .HasMaxLength(150);

        }
    }
}
