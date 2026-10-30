using Microsoft.Extensions.DependencyInjection;
using Recommenda.Application.Services;

namespace Recommenda.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IContentService, ContentService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IRatingService, RatingService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRecommendationLogService, RecommendationLogService>();

        return services;
    }
}
