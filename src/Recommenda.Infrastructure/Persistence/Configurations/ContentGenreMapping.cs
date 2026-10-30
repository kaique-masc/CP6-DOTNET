using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recommenda.Domain.Entities;
using Recommenda.Infrastructure.Persistence.Seed;

namespace Recommenda.Infrastructure.Persistence.Configurations;

// Tabela associativa do N:N entre Content e Genre
public class ContentGenreMapping : IEntityTypeConfiguration<ContentGenre>
{
    public void Configure(EntityTypeBuilder<ContentGenre> builder)
    {
        builder.ToTable("ContentGenres");

        builder.HasKey(cg => new { cg.ContentId, cg.GenreId });

        builder.Property(cg => cg.AssignedAt).IsRequired();

        builder.HasOne(cg => cg.Content)
            .WithMany(c => c.ContentGenres)
            .HasForeignKey(cg => cg.ContentId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // gênero em uso não pode ser apagado
        builder.HasOne(cg => cg.Genre)
            .WithMany(g => g.ContentGenres)
            .HasForeignKey(cg => cg.GenreId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(cg => cg.GenreId);

        builder.HasData(SeedData.ContentGenres);
    }
}
