using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recommenda.Domain.Entities;

namespace Recommenda.Infrastructure.Persistence.Configurations;

public class UserConfigurationMapping : IEntityTypeConfiguration<UserConfiguration>
{
    public void Configure(EntityTypeBuilder<UserConfiguration> builder)
    {
        builder.ToTable("UserConfigurations", t =>
            t.HasCheckConstraint("CK_UserConfigurations_MaxRecommendations",
                $"`MaxRecommendations` BETWEEN {UserConfiguration.MinRecommendations} AND {UserConfiguration.MaxRecommendationsLimit}"));

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.UserId).IsRequired();
        builder.HasIndex(c => c.UserId).IsUnique();

        builder.Property(c => c.PreferredLanguage)
            .HasMaxLength(UserConfiguration.LanguageMaxLength)
            .IsRequired();

        builder.Property(c => c.AllowAdultContent).IsRequired();
        builder.Property(c => c.ReceiveNotifications).IsRequired();
        builder.Property(c => c.MaxRecommendations).IsRequired();
        builder.Property(c => c.CreatedAt).IsRequired();
    }
}
