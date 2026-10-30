using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recommenda.Domain.Entities;

namespace Recommenda.Infrastructure.Persistence.Configurations;

public class RatingMapping : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.ToTable("Ratings", t =>
            t.HasCheckConstraint("CK_Ratings_Score", $"`Score` BETWEEN {Rating.MinScore} AND {Rating.MaxScore}"));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.Property(r => r.Score).IsRequired();
        builder.Property(r => r.Comment).HasMaxLength(Rating.CommentMaxLength);
        builder.Property(r => r.CreatedAt).IsRequired();

        builder.HasOne(r => r.User)
            .WithMany(u => u.Ratings)
            .HasForeignKey(r => r.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Content)
            .WithMany(c => c.Ratings)
            .HasForeignKey(r => r.ContentId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // um usuário avalia cada conteúdo uma única vez
        builder.HasIndex(r => new { r.UserId, r.ContentId }).IsUnique();
        builder.HasIndex(r => r.ContentId);
    }
}
