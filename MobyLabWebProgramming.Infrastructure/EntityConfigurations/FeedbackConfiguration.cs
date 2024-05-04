using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Infrastructure.EntityConfigurations;

/// <summary>
/// This is the entity configuration for the UserFile entity.
/// </summary>
public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.Property(e => e.Id)
            .IsRequired();
        builder.HasKey(x => x.Id);
        builder.Property(e => e.Score)
            .IsRequired();
        builder.Property(e => e.SiteDificulty)
           .HasMaxLength(255)
           .IsRequired();
        builder.Property(e => e.SiteGoal)
          .HasMaxLength(255)
          .IsRequired();
        builder.Property(e => e.RecommendToOthers)
           .IsRequired();
        builder.Property(e => e.Sugestion)
            .HasMaxLength(4095)
            .IsRequired(false);
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        builder.Property(e => e.UpdatedAt)
            .IsRequired();
    }

}