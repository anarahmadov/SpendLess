using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpendLess.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendLess.Identity.Configurations
{
    public class ApplicationTokenConfiguration : IEntityTypeConfiguration<ApplicationToken>
    {
        public void Configure(EntityTypeBuilder<ApplicationToken> builder)
        {
            // Define table name if needed
            builder.ToTable("ApplicationTokens");

            // Ignore Id property to be mapped
            builder.Ignore(t => t.Id);

            // Define primary key
            builder.HasKey(t => t.TokenString);

            // Set property configurations
            builder.Property(t => t.TokenString)
                .IsRequired()
                .HasMaxLength(256); // Adjust max length as needed

            builder.Property(t => t.UserId)
                .IsRequired();

            builder.Property(t => t.ExpirationDate)
                .IsRequired();

            // Configure foreign key
            builder.HasOne(t => t.User)
                .WithMany(u => u.ApplicationTokens)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Adjust DeleteBehavior if needed
        }
    }
}
