using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Infrastructure.Database.Configuration
{
    public class ContactRequestConfiguration : IEntityTypeConfiguration<ContactRequest>
    {
        public void Configure(EntityTypeBuilder<ContactRequest> builder)
        {
            builder.ToTable("ContactRequests");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);

            builder.Property(c => c.PhoneNumber).IsRequired().HasMaxLength(11).IsFixedLength();

            builder.Property(c => c.Message).IsRequired().HasMaxLength(2000);

            builder.Property(c => c.IsRead).HasDefaultValue(false);
        }
    }
}
