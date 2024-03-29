using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Infrastructure.EntityConfigurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.Property(e => e.Id) 
            .IsRequired(); 
        builder.HasKey(x => x.Id);
        builder.Property(e => e.Title)
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(e => e.Summary)
            .HasMaxLength(4095);
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        builder.Property(e => e.UpdatedAt)
            .IsRequired();
        builder.HasMany(e => e.Authors)
           .WithMany(e => e.Books)
           .UsingEntity(
               "BookAuthor",
               l => l.HasOne(typeof(Author)).WithMany().HasForeignKey("AuthorId").HasPrincipalKey(nameof(Author.Id)),
               r => r.HasOne(typeof(Book)).WithMany().HasForeignKey("BookId").HasPrincipalKey(nameof(Book.Id)),
               j => j.HasKey("BookId", "AuthorId")
           );
    }
}
