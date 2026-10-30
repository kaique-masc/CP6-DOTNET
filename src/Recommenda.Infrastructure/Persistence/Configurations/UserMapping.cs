using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recommenda.Domain.Entities;
using Recommenda.Infrastructure.Persistence.Seed;

namespace Recommenda.Infrastructure.Persistence.Configurations;

public class UserMapping : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedNever();

        builder.Property(u => u.Name)
            .HasMaxLength(User.NameMaxLength)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasMaxLength(User.EmailMaxLength)
            .IsRequired();

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.Password)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(u => u.BirthDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.UpdatedAt);

        builder.Ignore(u => u.Age);

        // 1:0..1 - o usuário pode ainda não ter configuração; a configuração sempre tem usuário
        builder.HasOne(u => u.Configuration)
            .WithOne(c => c.User)
            .HasForeignKey<UserConfiguration>(c => c.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(u => u.Ratings).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasData(SeedData.Users);
    }
}
