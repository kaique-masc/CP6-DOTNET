using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Recommenda.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Contents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Synopsis = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true),
                    Type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    ReleaseYear = table.Column<int>(type: "int", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    AgeRating = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contents", x => x.Id);
                    table.CheckConstraint("CK_Contents_DurationMinutes", "`DurationMinutes` > 0");
                    table.CheckConstraint("CK_Contents_ReleaseYear", "`ReleaseYear` >= 1888");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false),
                    Description = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "varchar(180)", maxLength: 180, nullable: false),
                    Password = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "date", nullable: false),
                    Role = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ContentGenres",
                columns: table => new
                {
                    ContentId = table.Column<Guid>(type: "char(36)", nullable: false),
                    GenreId = table.Column<Guid>(type: "char(36)", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentGenres", x => new { x.ContentId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_ContentGenres_Contents_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Contents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContentGenres_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ContentId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                    table.CheckConstraint("CK_Ratings_Score", "`Score` BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_Ratings_Contents_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Contents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ratings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    PreferredLanguage = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    AllowAdultContent = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ReceiveNotifications = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MaxRecommendations = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConfigurations", x => x.Id);
                    table.CheckConstraint("CK_UserConfigurations_MaxRecommendations", "`MaxRecommendations` BETWEEN 1 AND 50");
                    table.ForeignKey(
                        name: "FK_UserConfigurations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Contents",
                columns: new[] { "Id", "AgeRating", "CreatedAt", "DurationMinutes", "ReleaseYear", "Synopsis", "Title", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("08be18eb-0941-4605-9497-30a2b375def3"), 14, new DateTime(2026, 3, 8, 12, 0, 0, 0, DateTimeKind.Utc), 148, 2010, "Um ladrão especializado em invadir sonhos recebe a missão de implantar uma ideia.", "A Origem", "Movie", null },
                    { new Guid("0d54d2a0-7048-4201-a961-5d82af4a6124"), 10, new DateTime(2026, 3, 7, 12, 0, 0, 0, DateTimeKind.Utc), 169, 2014, "Exploradores viajam por um buraco de minhoca em busca de um novo lar para a humanidade.", "Interestelar", "Movie", null },
                    { new Guid("16fde398-28e5-4878-b33b-4cb310b2a425"), 12, new DateTime(2026, 3, 6, 12, 0, 0, 0, DateTimeKind.Utc), 104, 2000, "As aventuras de João Grilo e Chicó no sertão nordestino.", "O Auto da Compadecida", "Movie", null },
                    { new Guid("17e9aaf8-020b-4c62-8a25-f3a9ef958cee"), 16, new DateTime(2026, 3, 2, 12, 0, 0, 0, DateTimeKind.Utc), 127, 2018, "Após a morte da avó, uma família começa a descobrir segredos assustadores.", "Hereditário", "Movie", null },
                    { new Guid("6042e441-e8e7-49e6-bbb5-29b01941e1cd"), 16, new DateTime(2026, 3, 11, 12, 0, 0, 0, DateTimeKind.Utc), 47, 2008, "Um professor de química passa a produzir metanfetamina após um diagnóstico grave.", "Breaking Bad", "Series", null },
                    { new Guid("76ff5503-6912-4d06-b7cc-dd529ea784c5"), 0, new DateTime(2026, 3, 13, 12, 0, 0, 0, DateTimeKind.Utc), 125, 2001, "Uma menina entra em um mundo de espíritos e precisa salvar os pais.", "A Viagem de Chihiro", "Anime", null },
                    { new Guid("915c6f95-df5a-444d-87ea-858dcb022e16"), 16, new DateTime(2026, 3, 15, 12, 0, 0, 0, DateTimeKind.Utc), 120, 2015, "Em um deserto pós-apocalíptico, Max e Furiosa fogem de um tirano.", "Mad Max: Estrada da Fúria", "Movie", null },
                    { new Guid("a7722912-d4c0-4745-8523-c97a9da545a3"), 0, new DateTime(2026, 3, 14, 12, 0, 0, 0, DateTimeKind.Utc), 50, 2019, "Série documental sobre a vida selvagem e os impactos das mudanças climáticas.", "Nosso Planeta", "Documentary", null },
                    { new Guid("a87f5e0a-1c72-4e4f-ac99-8efaf3a072ba"), 0, new DateTime(2026, 3, 12, 12, 0, 0, 0, DateTimeKind.Utc), 95, 2015, "As emoções de uma garota tentam guiá-la durante uma mudança de cidade.", "Divertida Mente", "Movie", null },
                    { new Guid("bd87fe44-aac4-4ef9-a3c3-f83ae8abb0de"), 14, new DateTime(2026, 3, 3, 12, 0, 0, 0, DateTimeKind.Utc), 94, 2014, "Uma mãe viúva e o filho são assombrados por uma criatura saída de um livro infantil.", "O Babadook", "Movie", null },
                    { new Guid("dcdb2022-d30b-4c14-bfb8-c318cbceaeff"), 14, new DateTime(2026, 3, 4, 12, 0, 0, 0, DateTimeKind.Utc), 104, 2017, "Um jovem visita a família da namorada e percebe que algo está muito errado.", "Corra!", "Movie", null },
                    { new Guid("e87702a8-bc94-44d0-b729-00741e7df9ab"), 14, new DateTime(2026, 3, 10, 12, 0, 0, 0, DateTimeKind.Utc), 50, 2016, "Um grupo de amigos enfrenta forças sobrenaturais em uma pequena cidade.", "Stranger Things", "Series", null },
                    { new Guid("ed217a03-1db7-4203-a3d7-5bca094387d5"), 16, new DateTime(2026, 3, 5, 12, 0, 0, 0, DateTimeKind.Utc), 130, 2002, "A trajetória de jovens em uma comunidade do Rio de Janeiro entre os anos 60 e 80.", "Cidade de Deus", "Movie", null },
                    { new Guid("f124152f-16aa-4863-adb6-874eb60bdfb2"), 14, new DateTime(2026, 3, 1, 12, 0, 0, 0, DateTimeKind.Utc), 92, 2015, "Uma família puritana isolada na Nova Inglaterra do século XVII enfrenta forças misteriosas.", "A Bruxa", "Movie", null },
                    { new Guid("fef59306-fa9f-451f-b694-69a958677922"), 16, new DateTime(2026, 3, 9, 12, 0, 0, 0, DateTimeKind.Utc), 60, 2017, "O desaparecimento de crianças revela conexões entre quatro famílias e o tempo.", "Dark", "Series", null }
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("15b32338-5b8c-42e5-88ad-bb279b43582d"), new DateTime(2026, 3, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Filmes e séries de horror e suspense sobrenatural", "Terror", null },
                    { new Guid("3fe4e948-68e7-4d23-9b3b-d00c51d3417c"), new DateTime(2026, 3, 1, 12, 4, 0, 0, DateTimeKind.Utc), "Tecnologia, espaço e futuros possíveis", "Ficção Científica", null },
                    { new Guid("52f79c98-c343-491f-bc36-c2f8b0712f33"), new DateTime(2026, 3, 1, 12, 3, 0, 0, DateTimeKind.Utc), "Perseguições, lutas e muita adrenalina", "Ação", null },
                    { new Guid("58a22e31-200d-45ec-a8f4-da4353bea460"), new DateTime(2026, 3, 1, 12, 2, 0, 0, DateTimeKind.Utc), "Histórias com foco em conflitos e personagens", "Drama", null },
                    { new Guid("89887515-fd78-41bd-b273-708092b5d43e"), new DateTime(2026, 3, 1, 12, 5, 0, 0, DateTimeKind.Utc), "Produções animadas para todas as idades", "Animação", null },
                    { new Guid("99606127-b8f4-4108-84e5-11a5c15085b9"), new DateTime(2026, 3, 1, 12, 6, 0, 0, DateTimeKind.Utc), "Conteúdo baseado em fatos reais", "Documentário", null },
                    { new Guid("b0c88999-8890-4f78-85ff-87d8a3852a27"), new DateTime(2026, 3, 1, 12, 1, 0, 0, DateTimeKind.Utc), "Produções de humor", "Comédia", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDate", "CreatedAt", "Email", "Name", "Password", "Role", "UpdatedAt" },
                values: new object[] { new Guid("3029aef0-7192-453e-a2d3-1aea88640954"), new DateTime(1995, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 1, 12, 0, 0, 0, DateTimeKind.Utc), "admin@recommenda.com", "Administrador", "PBKDF2$100000$8ZaMlakxMFKcCM4ap3Cm2A==$QjbgdLYlVYZPrM5LutCNy3glAOdbZdGZ3L8jwe8uCXk=", "Admin", null });

            migrationBuilder.InsertData(
                table: "ContentGenres",
                columns: new[] { "ContentId", "GenreId", "AssignedAt" },
                values: new object[,]
                {
                    { new Guid("08be18eb-0941-4605-9497-30a2b375def3"), new Guid("3fe4e948-68e7-4d23-9b3b-d00c51d3417c"), new DateTime(2026, 3, 8, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("08be18eb-0941-4605-9497-30a2b375def3"), new Guid("52f79c98-c343-491f-bc36-c2f8b0712f33"), new DateTime(2026, 3, 8, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("0d54d2a0-7048-4201-a961-5d82af4a6124"), new Guid("3fe4e948-68e7-4d23-9b3b-d00c51d3417c"), new DateTime(2026, 3, 7, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("0d54d2a0-7048-4201-a961-5d82af4a6124"), new Guid("58a22e31-200d-45ec-a8f4-da4353bea460"), new DateTime(2026, 3, 7, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("16fde398-28e5-4878-b33b-4cb310b2a425"), new Guid("b0c88999-8890-4f78-85ff-87d8a3852a27"), new DateTime(2026, 3, 6, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("17e9aaf8-020b-4c62-8a25-f3a9ef958cee"), new Guid("15b32338-5b8c-42e5-88ad-bb279b43582d"), new DateTime(2026, 3, 2, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("17e9aaf8-020b-4c62-8a25-f3a9ef958cee"), new Guid("58a22e31-200d-45ec-a8f4-da4353bea460"), new DateTime(2026, 3, 2, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("6042e441-e8e7-49e6-bbb5-29b01941e1cd"), new Guid("58a22e31-200d-45ec-a8f4-da4353bea460"), new DateTime(2026, 3, 11, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("76ff5503-6912-4d06-b7cc-dd529ea784c5"), new Guid("89887515-fd78-41bd-b273-708092b5d43e"), new DateTime(2026, 3, 13, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("915c6f95-df5a-444d-87ea-858dcb022e16"), new Guid("3fe4e948-68e7-4d23-9b3b-d00c51d3417c"), new DateTime(2026, 3, 15, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("915c6f95-df5a-444d-87ea-858dcb022e16"), new Guid("52f79c98-c343-491f-bc36-c2f8b0712f33"), new DateTime(2026, 3, 15, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a7722912-d4c0-4745-8523-c97a9da545a3"), new Guid("99606127-b8f4-4108-84e5-11a5c15085b9"), new DateTime(2026, 3, 14, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a87f5e0a-1c72-4e4f-ac99-8efaf3a072ba"), new Guid("89887515-fd78-41bd-b273-708092b5d43e"), new DateTime(2026, 3, 12, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a87f5e0a-1c72-4e4f-ac99-8efaf3a072ba"), new Guid("b0c88999-8890-4f78-85ff-87d8a3852a27"), new DateTime(2026, 3, 12, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("bd87fe44-aac4-4ef9-a3c3-f83ae8abb0de"), new Guid("15b32338-5b8c-42e5-88ad-bb279b43582d"), new DateTime(2026, 3, 3, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("dcdb2022-d30b-4c14-bfb8-c318cbceaeff"), new Guid("15b32338-5b8c-42e5-88ad-bb279b43582d"), new DateTime(2026, 3, 4, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("e87702a8-bc94-44d0-b729-00741e7df9ab"), new Guid("15b32338-5b8c-42e5-88ad-bb279b43582d"), new DateTime(2026, 3, 10, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("e87702a8-bc94-44d0-b729-00741e7df9ab"), new Guid("3fe4e948-68e7-4d23-9b3b-d00c51d3417c"), new DateTime(2026, 3, 10, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("ed217a03-1db7-4203-a3d7-5bca094387d5"), new Guid("52f79c98-c343-491f-bc36-c2f8b0712f33"), new DateTime(2026, 3, 5, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("ed217a03-1db7-4203-a3d7-5bca094387d5"), new Guid("58a22e31-200d-45ec-a8f4-da4353bea460"), new DateTime(2026, 3, 5, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("f124152f-16aa-4863-adb6-874eb60bdfb2"), new Guid("15b32338-5b8c-42e5-88ad-bb279b43582d"), new DateTime(2026, 3, 1, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("f124152f-16aa-4863-adb6-874eb60bdfb2"), new Guid("58a22e31-200d-45ec-a8f4-da4353bea460"), new DateTime(2026, 3, 1, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("fef59306-fa9f-451f-b694-69a958677922"), new Guid("3fe4e948-68e7-4d23-9b3b-d00c51d3417c"), new DateTime(2026, 3, 9, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("fef59306-fa9f-451f-b694-69a958677922"), new Guid("58a22e31-200d-45ec-a8f4-da4353bea460"), new DateTime(2026, 3, 9, 12, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentGenres_GenreId",
                table: "ContentGenres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_CreatedAt_Id",
                table: "Contents",
                columns: new[] { "CreatedAt", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Contents_Title",
                table: "Contents",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Genres_Name",
                table: "Genres",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_ContentId",
                table: "Ratings",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_UserId_ContentId",
                table: "Ratings",
                columns: new[] { "UserId", "ContentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserConfigurations_UserId",
                table: "UserConfigurations",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentGenres");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "UserConfigurations");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Contents");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
