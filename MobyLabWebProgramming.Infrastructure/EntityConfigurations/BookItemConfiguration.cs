using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Infrastructure.EntityConfigurations;

public class BookItemConfiguration : IEntityTypeConfiguration<BookItem>
{
    public void Configure(EntityTypeBuilder<BookItem> builder)
    {
        builder.Property(e => e.Id)
            .IsRequired();
        builder.HasKey(x => x.Id);
        builder.Property(e => e.BarCode)
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.HasOne(e => e.Book) 
            .WithMany(e => e.BookItems) // This provides the reverse mapping for the one-to-many relation. 
            .HasForeignKey(e => e.BookId) 
            .HasPrincipalKey(e => e.Id) 
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade); 
    }
}
