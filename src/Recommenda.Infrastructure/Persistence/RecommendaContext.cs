using Microsoft.EntityFrameworkCore;
using Recommenda.Domain.Entities;
using Recommenda.Infrastructure.Persistence.Converters;

namespace Recommenda.Infrastructure.Persistence;

public class RecommendaContext : DbContext
{
    public RecommendaContext(DbContextOptions<RecommendaContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserConfiguration> UserConfigurations => Set<UserConfiguration>();
    public DbSet<Content> Contents => Set<Content>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<ContentGenre> ContentGenres => Set<ContentGenre>();
    public DbSet<Rating> Ratings => Set<Rating>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
        configurationBuilder.Properties<DateOnly>().HaveConversion<DateOnlyConverter>().HaveColumnType("date");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RecommendaContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
