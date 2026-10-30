using Recommenda.Application.DTOs.Contents;
using Recommenda.Application.DTOs.Genres;
using Recommenda.Application.DTOs.Ratings;
using Recommenda.Application.DTOs.RecommendationLogs;
using Recommenda.Application.DTOs.Users;
using Recommenda.Domain.Entities;

namespace Recommenda.Application.Mappings;

public static class MappingExtensions
{
    public static UserResponse ToResponse(this User user) =>
        new(user.Id, user.Name, user.Email, user.BirthDate, user.Age, user.Role.ToString(), user.CreatedAt);

    public static UserConfigurationResponse ToResponse(this UserConfiguration configuration) =>
        new(configuration.Id,
            configuration.UserId,
            configuration.PreferredLanguage,
            configuration.AllowAdultContent,
            configuration.ReceiveNotifications,
            configuration.MaxRecommendations,
            configuration.UpdatedAt);

    public static ContentResponse ToResponse(this Content content) =>
        new(content.Id,
            content.Title,
            content.Synopsis,
            content.Type.ToString(),
            content.ReleaseYear,
            content.DurationMinutes,
            content.AgeRating,
            content.CreatedAt);

    public static ContentDetailResponse ToDetailResponse(this Content content) =>
        new(content.Id,
            content.Title,
            content.Synopsis,
            content.Type.ToString(),
            content.ReleaseYear,
            content.DurationMinutes,
            content.AgeRating,
            content.CreatedAt,
            content.UpdatedAt,
            content.ContentGenres
                .Where(cg => cg.Genre is not null)
                .Select(cg => new GenreSummary(cg.GenreId, cg.Genre!.Name))
                .OrderBy(g => g.Name)
                .ToList());

    public static GenreResponse ToResponse(this Genre genre) =>
        new(genre.Id, genre.Name, genre.Description, genre.CreatedAt);

    public static RatingResponse ToResponse(this Rating rating) =>
        new(rating.Id, rating.UserId, rating.ContentId, rating.Score, rating.Comment, rating.CreatedAt);

    public static RecommendationLogItem ToItem(this RecommendationLog log) =>
        new(log.Id, log.UserId, log.Query, log.Titles, log.CreatedAt);
}
