using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recommenda.Domain.Entities;
using Recommenda.Infrastructure.Persistence.Seed;

namespace Recommenda.Infrastructure.Persistence.Configurations;

public class ContentMapping : IEntityTypeConfiguration<Content>
{
    public void Configure(EntityTypeBuilder<Content> builder)
    {
        builder.ToTable("Contents", t =>
        {
            t.HasCheckConstraint("CK_Contents_DurationMinutes", "`DurationMinutes` > 0");
            t.HasCheckConstraint("CK_Contents_ReleaseYear", $"`ReleaseYear` >= {Content.FirstReleaseYear}");
        });

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.Title)
            .HasMaxLength(Content.TitleMaxLength)
            .IsRequired();

        builder.Property(c => c.Synopsis)
            .HasMaxLength(Content.SynopsisMaxLength);

        builder.Property(c => c.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.ReleaseYear).IsRequired();
        builder.Property(c => c.DurationMinutes).IsRequired();
        builder.Property(c => c.AgeRating).IsRequired();
        builder.Property(c => c.CreatedAt).IsRequired();

        builder.Ignore(c => c.IsAdultOnly);

        // índice usado na ordenação da listagem paginada (v2)
        builder.HasIndex(c => new { c.CreatedAt, c.Id });
        builder.HasIndex(c => c.Title);

        builder.Navigation(c => c.ContentGenres).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(c => c.Ratings).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasData(SeedData.Contents);
    }
}
