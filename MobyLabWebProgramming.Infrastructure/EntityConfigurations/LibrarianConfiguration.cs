using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Infrastructure.EntityConfigurations;

/// <summary>
/// This is the entity configuration for the UserFile entity.
/// </summary>
public class LibrarianConfiguration : IEntityTypeConfiguration<Librarian>
{
    public void Configure(EntityTypeBuilder<Librarian> builder)
    {
        builder.Property(e => e.Id)
            .IsRequired();
        builder.HasKey(x => x.Id);
        builder.Property(e => e.FirstName)
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(e => e.LastName)
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        builder.Property(e => e.UpdatedAt)
            .IsRequired();
        builder.HasOne(e => e.User) // This specifies a one-to-many relation.
          .WithOne(e => e.Librarian) // This provides the reverse mapping for the one-to-many relation. 
          .HasForeignKey<Librarian>(e => e.UserId) // Here the foreign key column is specified.
         .IsRequired();
    }

}