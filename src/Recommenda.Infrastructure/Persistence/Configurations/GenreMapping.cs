using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recommenda.Domain.Entities;
using Recommenda.Infrastructure.Persistence.Seed;

namespace Recommenda.Infrastructure.Persistence.Configurations;

public class GenreMapping : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable("Genres");

        builder.HasKey(g => g.Id);
        builder.Property(g => g.Id).ValueGeneratedNever();

        builder.Property(g => g.Name)
            .HasMaxLength(Genre.NameMaxLength)
            .IsRequired();

        builder.HasIndex(g => g.Name).IsUnique();

        builder.Property(g => g.Description)
            .HasMaxLength(Genre.DescriptionMaxLength);

        builder.Property(g => g.CreatedAt).IsRequired();

        builder.Navigation(g => g.ContentGenres).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasData(SeedData.Genres);
    }
}
