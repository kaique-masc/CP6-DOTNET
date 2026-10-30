namespace Recommenda.Infrastructure.Persistence.Seed;

// Dados iniciais aplicados pela migration (gêneros, catálogo de exemplo e um administrador).
internal static class SeedData
{
    private static readonly DateTime BaseDate = new(2026, 3, 1, 12, 0, 0, DateTimeKind.Utc);

    public static readonly Guid Terror = Guid.Parse("15b32338-5b8c-42e5-88ad-bb279b43582d");
    public static readonly Guid Comedia = Guid.Parse("b0c88999-8890-4f78-85ff-87d8a3852a27");
    public static readonly Guid Drama = Guid.Parse("58a22e31-200d-45ec-a8f4-da4353bea460");
    public static readonly Guid Acao = Guid.Parse("52f79c98-c343-491f-bc36-c2f8b0712f33");
    public static readonly Guid FiccaoCientifica = Guid.Parse("3fe4e948-68e7-4d23-9b3b-d00c51d3417c");
    public static readonly Guid Animacao = Guid.Parse("89887515-fd78-41bd-b273-708092b5d43e");
    public static readonly Guid Documentario = Guid.Parse("99606127-b8f4-4108-84e5-11a5c15085b9");

    public static readonly Guid AdminId = Guid.Parse("3029aef0-7192-453e-a2d3-1aea88640954");

    // senha: admin1234
    private const string AdminPasswordHash = "PBKDF2$100000$8ZaMlakxMFKcCM4ap3Cm2A==$QjbgdLYlVYZPrM5LutCNy3glAOdbZdGZ3L8jwe8uCXk=";

    public static object[] Genres =>
    [
        Genre(Terror, "Terror", "Filmes e séries de horror e suspense sobrenatural", 0),
        Genre(Comedia, "Comédia", "Produções de humor", 1),
        Genre(Drama, "Drama", "Histórias com foco em conflitos e personagens", 2),
        Genre(Acao, "Ação", "Perseguições, lutas e muita adrenalina", 3),
        Genre(FiccaoCientifica, "Ficção Científica", "Tecnologia, espaço e futuros possíveis", 4),
        Genre(Animacao, "Animação", "Produções animadas para todas as idades", 5),
        Genre(Documentario, "Documentário", "Conteúdo baseado em fatos reais", 6)
    ];

    public static object[] Users =>
    [
        new
        {
            Id = AdminId,
            Name = "Administrador",
            Email = "admin@recommenda.com",
            Password = AdminPasswordHash,
            BirthDate = new DateOnly(1995, 1, 15),
            Role = Domain.Enums.UserRole.Admin,
            CreatedAt = BaseDate,
            UpdatedAt = (DateTime?)null
        }
    ];

    private static readonly (Guid Id, string Title, string Synopsis, Domain.Enums.ContentType Type, int Year, int Duration, int Age, Guid[] Genres)[] Catalog =
    [
        (Guid.Parse("f124152f-16aa-4863-adb6-874eb60bdfb2"), "A Bruxa", "Uma família puritana isolada na Nova Inglaterra do século XVII enfrenta forças misteriosas.", Domain.Enums.ContentType.Movie, 2015, 92, 14, [Terror, Drama]),
        (Guid.Parse("17e9aaf8-020b-4c62-8a25-f3a9ef958cee"), "Hereditário", "Após a morte da avó, uma família começa a descobrir segredos assustadores.", Domain.Enums.ContentType.Movie, 2018, 127, 16, [Terror, Drama]),
        (Guid.Parse("bd87fe44-aac4-4ef9-a3c3-f83ae8abb0de"), "O Babadook", "Uma mãe viúva e o filho são assombrados por uma criatura saída de um livro infantil.", Domain.Enums.ContentType.Movie, 2014, 94, 14, [Terror]),
        (Guid.Parse("dcdb2022-d30b-4c14-bfb8-c318cbceaeff"), "Corra!", "Um jovem visita a família da namorada e percebe que algo está muito errado.", Domain.Enums.ContentType.Movie, 2017, 104, 14, [Terror]),
        (Guid.Parse("ed217a03-1db7-4203-a3d7-5bca094387d5"), "Cidade de Deus", "A trajetória de jovens em uma comunidade do Rio de Janeiro entre os anos 60 e 80.", Domain.Enums.ContentType.Movie, 2002, 130, 16, [Drama, Acao]),
        (Guid.Parse("16fde398-28e5-4878-b33b-4cb310b2a425"), "O Auto da Compadecida", "As aventuras de João Grilo e Chicó no sertão nordestino.", Domain.Enums.ContentType.Movie, 2000, 104, 12, [Comedia]),
        (Guid.Parse("0d54d2a0-7048-4201-a961-5d82af4a6124"), "Interestelar", "Exploradores viajam por um buraco de minhoca em busca de um novo lar para a humanidade.", Domain.Enums.ContentType.Movie, 2014, 169, 10, [FiccaoCientifica, Drama]),
        (Guid.Parse("08be18eb-0941-4605-9497-30a2b375def3"), "A Origem", "Um ladrão especializado em invadir sonhos recebe a missão de implantar uma ideia.", Domain.Enums.ContentType.Movie, 2010, 148, 14, [FiccaoCientifica, Acao]),
        (Guid.Parse("fef59306-fa9f-451f-b694-69a958677922"), "Dark", "O desaparecimento de crianças revela conexões entre quatro famílias e o tempo.", Domain.Enums.ContentType.Series, 2017, 60, 16, [FiccaoCientifica, Drama]),
        (Guid.Parse("e87702a8-bc94-44d0-b729-00741e7df9ab"), "Stranger Things", "Um grupo de amigos enfrenta forças sobrenaturais em uma pequena cidade.", Domain.Enums.ContentType.Series, 2016, 50, 14, [FiccaoCientifica, Terror]),
        (Guid.Parse("6042e441-e8e7-49e6-bbb5-29b01941e1cd"), "Breaking Bad", "Um professor de química passa a produzir metanfetamina após um diagnóstico grave.", Domain.Enums.ContentType.Series, 2008, 47, 16, [Drama]),
        (Guid.Parse("a87f5e0a-1c72-4e4f-ac99-8efaf3a072ba"), "Divertida Mente", "As emoções de uma garota tentam guiá-la durante uma mudança de cidade.", Domain.Enums.ContentType.Movie, 2015, 95, 0, [Animacao, Comedia]),
        (Guid.Parse("76ff5503-6912-4d06-b7cc-dd529ea784c5"), "A Viagem de Chihiro", "Uma menina entra em um mundo de espíritos e precisa salvar os pais.", Domain.Enums.ContentType.Anime, 2001, 125, 0, [Animacao]),
        (Guid.Parse("a7722912-d4c0-4745-8523-c97a9da545a3"), "Nosso Planeta", "Série documental sobre a vida selvagem e os impactos das mudanças climáticas.", Domain.Enums.ContentType.Documentary, 2019, 50, 0, [Documentario]),
        (Guid.Parse("915c6f95-df5a-444d-87ea-858dcb022e16"), "Mad Max: Estrada da Fúria", "Em um deserto pós-apocalíptico, Max e Furiosa fogem de um tirano.", Domain.Enums.ContentType.Movie, 2015, 120, 16, [Acao, FiccaoCientifica])
    ];

    public static object[] Contents => Catalog
        .Select((c, index) => (object)new
        {
            c.Id,
            c.Title,
            Synopsis = c.Synopsis,
            c.Type,
            ReleaseYear = c.Year,
            DurationMinutes = c.Duration,
            AgeRating = c.Age,
            CreatedAt = BaseDate.AddDays(index),
            UpdatedAt = (DateTime?)null
        })
        .ToArray();

    public static object[] ContentGenres => Catalog
        .SelectMany((c, index) => c.Genres.Select(g => (object)new
        {
            ContentId = c.Id,
            GenreId = g,
            AssignedAt = BaseDate.AddDays(index)
        }))
        .ToArray();

    private static object Genre(Guid id, string name, string description, int order) => new
    {
        Id = id,
        Name = name,
        Description = (string?)description,
        CreatedAt = BaseDate.AddMinutes(order),
        UpdatedAt = (DateTime?)null
    };
}
