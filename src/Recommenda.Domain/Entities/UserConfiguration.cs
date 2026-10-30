using Recommenda.Domain.Common;
using Recommenda.Domain.Exceptions;

namespace Recommenda.Domain.Entities;

public class UserConfiguration : BaseEntity
{
    public const int MinRecommendations = 1;
    public const int MaxRecommendationsLimit = 50;
    public const int LanguageMaxLength = 10;

    protected UserConfiguration()
    {
    }

    public UserConfiguration(Guid userId, string preferredLanguage, bool allowAdultContent, bool receiveNotifications, int maxRecommendations)
    {
        if (userId == Guid.Empty)
            throw new DomainException("A configuração precisa estar vinculada a um usuário.");

        UserId = userId;
        Apply(preferredLanguage, allowAdultContent, receiveNotifications, maxRecommendations);
    }

    public Guid UserId { get; private set; }
    public string PreferredLanguage { get; private set; } = "pt-BR";
    public bool AllowAdultContent { get; private set; }
    public bool ReceiveNotifications { get; private set; } = true;
    public int MaxRecommendations { get; private set; } = 10;

    public User? User { get; private set; }

    public void Update(string preferredLanguage, bool allowAdultContent, bool receiveNotifications, int maxRecommendations)
    {
        Apply(preferredLanguage, allowAdultContent, receiveNotifications, maxRecommendations);
        Touch();
    }

    private void Apply(string preferredLanguage, bool allowAdultContent, bool receiveNotifications, int maxRecommendations)
    {
        if (string.IsNullOrWhiteSpace(preferredLanguage) || preferredLanguage.Trim().Length > LanguageMaxLength)
            throw new DomainException("O idioma preferido é obrigatório (ex.: pt-BR).");

        if (maxRecommendations < MinRecommendations || maxRecommendations > MaxRecommendationsLimit)
            throw new DomainException($"A quantidade de recomendações deve estar entre {MinRecommendations} e {MaxRecommendationsLimit}.");

        PreferredLanguage = preferredLanguage.Trim();
        AllowAdultContent = allowAdultContent;
        ReceiveNotifications = receiveNotifications;
        MaxRecommendations = maxRecommendations;
    }
}
