CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    PRIMARY KEY (`MigrationId`)
);

START TRANSACTION;
IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261030003603_InitialCreate')
BEGIN
    CREATE TABLE `Contents` (
        `Id` char(36) NOT NULL,
        `Title` varchar(200) NOT NULL,
        `Synopsis` varchar(2000) NULL,
        `Type` varchar(20) NOT NULL,
        `ReleaseYear` int NOT NULL,
        `DurationMinutes` int NOT NULL,
        `AgeRating` int NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        PRIMARY KEY (`Id`),
        CONSTRAINT `CK_Contents_DurationMinutes` CHECK (`DurationMinutes` > 0),
        CONSTRAINT `CK_Contents_ReleaseYear` CHECK (`ReleaseYear` >= 1888)
    );
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261030003603_InitialCreate')
BEGIN
    CREATE TABLE `Genres` (
        `Id` char(36) NOT NULL,
        `Name` varchar(60) NOT NULL,
        `Description` varchar(250) NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        PRIMARY KEY (`Id`)
    );
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261030003603_InitialCreate')
BEGIN
    CREATE TABLE `Users` (
        `Id` char(36) NOT NULL,
        `Name` varchar(120) NOT NULL,
        `Email` varchar(180) NOT NULL,
        `Password` varchar(256) NOT NULL,
        `BirthDate` date NOT NULL,
        `Role` varchar(20) NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        PRIMARY KEY (`Id`)
    );
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261030003603_InitialCreate')
BEGIN
    CREATE TABLE `ContentGenres` (
        `ContentId` char(36) NOT NULL,
        `GenreId` char(36) NOT NULL,
        `AssignedAt` datetime(6) NOT NULL,
        PRIMARY KEY (`ContentId`, `GenreId`),
        CONSTRAINT `FK_ContentGenres_Contents_ContentId` FOREIGN KEY (`ContentId`) REFERENCES `Contents` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_ContentGenres_Genres_GenreId` FOREIGN KEY (`GenreId`) REFERENCES `Genres` (`Id`) ON DELETE RESTRICT
    );
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261030003603_InitialCreate')
BEGIN
    CREATE TABLE `Ratings` (
        `Id` char(36) NOT NULL,
        `UserId` char(36) NOT NULL,
        `ContentId` char(36) NOT NULL,
        `Score` int NOT NULL,
        `Comment` varchar(500) NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        PRIMARY KEY (`Id`),
        CONSTRAINT `CK_Ratings_Score` CHECK (`Score` BETWEEN 1 AND 5),
        CONSTRAINT `FK_Ratings_Contents_ContentId` FOREIGN KEY (`ContentId`) REFERENCES `Contents` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_Ratings_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
    );
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261030003603_InitialCreate')
BEGIN
    CREATE TABLE `UserConfigurations` (
        `Id` char(36) NOT NULL,
        `UserId` char(36) NOT NULL,
        `PreferredLanguage` varchar(10) NOT NULL,
        `AllowAdultContent` tinyint(1) NOT NULL,
        `ReceiveNotifications` tinyint(1) NOT NULL,
        `MaxRecommendations` int NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatedAt` datetime(6) NULL,
        PRIMARY KEY (`Id`),
        CONSTRAINT `CK_UserConfigurations_MaxRecommendations` CHECK (`MaxRecommendations` BETWEEN 1 AND 50),
        CONSTRAINT `FK_UserConfigurations_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
    );
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261030003603_InitialCreate')
BEGIN
    INSERT INTO `Contents` (`Id`, `AgeRating`, `CreatedAt`, `DurationMinutes`, `ReleaseYear`, `Synopsis`, `Title`, `Type`, `UpdatedAt`)
    VALUES ('08be18eb-0941-4605-9497-30a2b375def3', 14, '2026-03-08 12:00:00.000000', 148, 2010, 'Um ladrão especializado em invadir sonhos recebe a missão de implantar uma ideia.', 'A Origem', 'Movie', NULL);
    SELECT ROW_COUNT();

    INSERT INTO `Contents` (`Id`, `AgeRating`, `CreatedAt`, `DurationMinutes`, `ReleaseYear`, `Synopsis`, `Title`, `Type`, `UpdatedAt`)
    VALUES ('0d54d2a0-7048-4201-a961-5d82af4a6124', 10, '2026-03-07 12:00:00.000000', 169, 2014, 'Exploradores viajam por um buraco de minhoca em busca de um novo lar para a humanidade.', 'Interestelar', 'Movie', NULL);
    SELECT ROW_COUNT();

    INSERT INTO `Contents` (`Id`, `AgeRating`, `CreatedAt`, `DurationMinutes`, `ReleaseYear`, `Synopsis`, `Title`, `Type`, `UpdatedAt`)
    VALUES ('16fde398-28e5-4878-b33b-4cb310b2a425', 12, '2026-03-06 12:00:00.000000', 104, 2000, 'As aventuras de João Grilo e Chicó no sertão nordestino.', 'O Auto da Compadecida', 'Movie', NULL);
    SELECT ROW_COUNT();

    INSERT INTO `Contents` (`Id`, `AgeRating`, `CreatedAt`, `DurationMinutes`, `ReleaseYear`, `Synopsis`, `Title`, `Type`, `UpdatedAt`)
    VALUES ('17e9aaf8-020b-4c62-8a25-f3a9ef958cee', 16, '2026-03-02 12:00:00.000000', 127, 2018, 'Após a morte da avó, uma família começa a descobrir segredos assustadores.', 'Hereditário', 'Movie', NULL);
    SELECT ROW_COUNT();

    INSERT INTO `Contents` (`Id`, `AgeRating`, `CreatedAt`, `DurationMinutes`, `ReleaseYear`, `Synopsis`, `Title`, `Type`, `UpdatedAt`)
    VALUES ('6042e441-e8e7-49e6-bbb5-29b01941e1cd', 16, '2026-03-11 12:00:00.000000', 47, 2008, 'Um professor de química passa a produzir metanfetamina após um diagnóstico grave.', 'Breaking Bad', 'Series', NULL);
    SELECT ROW_COUNT();

    INSERT INTO `Contents` (`Id`, `AgeRating`, `CreatedAt`, `DurationMinutes`, `ReleaseYear`, `Synopsis`, `Title`, `Type`, `UpdatedAt`)
    VALUES ('76ff5503-6912-4d06-b7cc-dd529ea784c5', 0, '2026-03-13 12:00:00.000000', 125, 2001, 'Uma menina entra em um mundo de espíritos e precisa salvar os pais.', 'A Viagem de Chihiro', 'Anime', NULL);
    SELECT ROW_COUNT();

    INSERT INTO `Contents` (`Id`, `AgeRating`, `CreatedAt`, `DurationMinutes`, `ReleaseYear`, `Synopsis`, `Title`, `Type`, `UpdatedAt`)
    VALUES ('915c6f95-df5a-444d-87ea-858dcb022e16', 16, '2026-03-15 12:00:00.000000', 120, 2015, 'Em um deserto pós-apocalíptico, Max e Furiosa fogem de um tirano.', 'Mad Max: Estrada da Fúria', 'Movie', NULL);
    SELECT ROW_COUNT();

    INSERT INTO `Contents` (`Id`, `AgeRating`, `CreatedAt`, `DurationMinutes`, `ReleaseYear`, `Synopsis`, `Title`, `Type`, `UpdatedAt`)
    VALUES ('a7722912-d4c0-4745-8523-c97a9da545a3', 0, '2026-03-14 12:00:00.000000', 50, 2019, 'Série documental sobre a vida selvagem e os impactos das mudanças climáticas.', 'Nosso Planeta', 'Documentary', NULL);
    SELECT ROW_COUNT();

    INSERT INTO `Contents` (`Id`, `AgeRating`, `CreatedAt`, `DurationMinutes`, `ReleaseYear`, `Synopsis`, `Title`, `Type`, `UpdatedAt`)
    VALUES ('a87f5e0a-1c72-4e4f-ac99-8efaf3a072ba', 0, '2026-03-12 12:00:00.000000', 95, 2015, 'As emoções de uma garota tentam guiá-la durante uma mudança de cidade.', 'Divertida Mente', 'Movie', NULL);
    SELECT ROW_COUNT();

    INSERT INTO `Contents` (`Id`, `AgeRating`, `CreatedAt`, `DurationMinutes`, `ReleaseYear`, `Synopsis`, `Title`, `Type`, `UpdatedAt`)
    VALUES ('bd87fe44-aac4-4ef9-a3c3-f83ae8abb0de', 14, '2026-03-03 12:00:00.000000', 94, 2014, 'Uma mãe viúva e o filho são assombrados por uma criatura saída de um livro infantil.', 'O Babadook', 'Movie', NULL);
    SELECT ROW_COUNT();

    INSERT INTO `Contents` (`Id`, `AgeRating`, `CreatedAt`, `DurationMinutes`, `ReleaseYear`, `Synopsis`, `Title`, `Type`, `UpdatedAt`)
    VALUES ('dcdb2022-d30b-4c14-bfb8-c318cbceaeff', 14, '2026-03-04 12:00:00.000000', 104, 2017, 'Um jovem visita a família da namorada e percebe que algo está muito errado.', 'Corra!', 'Movie', NULL);
    SELECT ROW_COUNT();

    INSERT INTO `Contents` (`Id`, `AgeRating`, `CreatedAt`, `DurationMinutes`, `ReleaseYear`, `Synopsis`, `Title`, `Type`, `UpdatedAt`)
    VALUES ('e87702a8-bc94-44d0-b729-00741e7df9ab', 14, '2026-03-10 12:00:00.000000', 50, 2016, 'Um grupo de amigos enfrenta forças sobrenaturais em uma pequena cidade.', 'Stranger Things', 'Series', NULL);
    SELECT ROW_COUNT();

    INSERT INTO `Contents` (`Id`, `AgeRating`, `CreatedAt`, `DurationMinutes`, `ReleaseYear`, `Synopsis`, `Title`, `Type`, `UpdatedAt`)
    VALUES ('ed217a03-1db7-4203-a3d7-5bca094387d5', 16, '2026-03-05 12:00:00.000000', 130, 2002, 'A trajetória de jovens em uma comunidade do Rio de Janeiro entre os anos 60 e 80.', 'Cidade de Deus', 'Movie', NULL);
    SELECT ROW_COUNT();

    INSERT INTO `Contents` (`Id`, `AgeRating`, `CreatedAt`, `DurationMinutes`, `ReleaseYear`, `Synopsis`, `Title`, `Type`, `UpdatedAt`)
    VALUES ('f124152f-16aa-4863-adb6-874eb60bdfb2', 14, '2026-03-01 12:00:00.000000', 92, 2015, 'Uma família puritana isolada na Nova Inglaterra do século XVII enfrenta forças misteriosas.', 'A Bruxa', 'Movie', NULL);
    SELECT ROW_COUNT();

    INSERT INTO `Contents` (`Id`, `AgeRating`, `CreatedAt`, `DurationMinutes`, `ReleaseYear`, `Synopsis`, `Title`, `Type`, `UpdatedAt`)
    VALUES ('fef59306-fa9f-451f-b694-69a958677922', 16, '2026-03-09 12:00:00.000000', 60, 2017, 'O desaparecimento de crianças revela conexões entre quatro famílias e o tempo.', 'Dark', 'Series', NULL);
    SELECT ROW_COUNT();

END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261030003603_InitialCreate')
BEGIN
    INSERT INTO `Genres` (`Id`, `CreatedAt`, `Description`, `Name`, `UpdatedAt`)
    VALUES ('15b32338-5b8c-42e5-88ad-bb279b43582d', '2026-03-01 12:00:00.000000', 'Filmes e séries de horror e suspense sobrenatural', 'Terror', NULL);
    SELECT ROW_COUNT();

    INSERT INTO `Genres` (`Id`, `CreatedAt`, `Description`, `Name`, `UpdatedAt`)
    VALUES ('3fe4e948-68e7-4d23-9b3b-d00c51d3417c', '2026-03-01 12:04:00.000000', 'Tecnologia, espaço e futuros possíveis', 'Ficção Científica', NULL);
    SELECT ROW_COUNT();

    INSERT INTO `Genres` (`Id`, `CreatedAt`, `Description`, `Name`, `UpdatedAt`)
    VALUES ('52f79c98-c343-491f-bc36-c2f8b0712f33', '2026-03-01 12:03:00.000000', 'Perseguições, lutas e muita adrenalina', 'Ação', NULL);
    SELECT ROW_COUNT();

    INSERT INTO `Genres` (`Id`, `CreatedAt`, `Description`, `Name`, `UpdatedAt`)
    VALUES ('58a22e31-200d-45ec-a8f4-da4353bea460', '2026-03-01 12:02:00.000000', 'Histórias com foco em conflitos e personagens', 'Drama', NULL);
    SELECT ROW_COUNT();

    INSERT INTO `Genres` (`Id`, `CreatedAt`, `Description`, `Name`, `UpdatedAt`)
    VALUES ('89887515-fd78-41bd-b273-708092b5d43e', '2026-03-01 12:05:00.000000', 'Produções animadas para todas as idades', 'Animação', NULL);
    SELECT ROW_COUNT();

    INSERT INTO `Genres` (`Id`, `CreatedAt`, `Description`, `Name`, `UpdatedAt`)
    VALUES ('99606127-b8f4-4108-84e5-11a5c15085b9', '2026-03-01 12:06:00.000000', 'Conteúdo baseado em fatos reais', 'Documentário', NULL);
    SELECT ROW_COUNT();

    INSERT INTO `Genres` (`Id`, `CreatedAt`, `Description`, `Name`, `UpdatedAt`)
    VALUES ('b0c88999-8890-4f78-85ff-87d8a3852a27', '2026-03-01 12:01:00.000000', 'Produções de humor', 'Comédia', NULL);
    SELECT ROW_COUNT();

END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261030003603_InitialCreate')
BEGIN
    INSERT INTO `Users` (`Id`, `BirthDate`, `CreatedAt`, `Email`, `Name`, `Password`, `Role`, `UpdatedAt`)
    VALUES ('3029aef0-7192-453e-a2d3-1aea88640954', DATE '1995-01-15', '2026-03-01 12:00:00.000000', 'admin@recommenda.com', 'Administrador', 'PBKDF2$100000$8ZaMlakxMFKcCM4ap3Cm2A==$QjbgdLYlVYZPrM5LutCNy3glAOdbZdGZ3L8jwe8uCXk=', 'Admin', NULL);
    SELECT ROW_COUNT();

END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261030003603_InitialCreate')
BEGIN
    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('08be18eb-0941-4605-9497-30a2b375def3', '3fe4e948-68e7-4d23-9b3b-d00c51d3417c', '2026-03-08 12:00:00.000000');
    SELECT ROW_COUNT();

    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('08be18eb-0941-4605-9497-30a2b375def3', '52f79c98-c343-491f-bc36-c2f8b0712f33', '2026-03-08 12:00:00.000000');
    SELECT ROW_COUNT();

    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('0d54d2a0-7048-4201-a961-5d82af4a6124', '3fe4e948-68e7-4d23-9b3b-d00c51d3417c', '2026-03-07 12:00:00.000000');
    SELECT ROW_COUNT();

    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('0d54d2a0-7048-4201-a961-5d82af4a6124', '58a22e31-200d-45ec-a8f4-da4353bea460', '2026-03-07 12:00:00.000000');
    SELECT ROW_COUNT();

    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('16fde398-28e5-4878-b33b-4cb310b2a425', 'b0c88999-8890-4f78-85ff-87d8a3852a27', '2026-03-06 12:00:00.000000');
    SELECT ROW_COUNT();

    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('17e9aaf8-020b-4c62-8a25-f3a9ef958cee', '15b32338-5b8c-42e5-88ad-bb279b43582d', '2026-03-02 12:00:00.000000');
    SELECT ROW_COUNT();

    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('17e9aaf8-020b-4c62-8a25-f3a9ef958cee', '58a22e31-200d-45ec-a8f4-da4353bea460', '2026-03-02 12:00:00.000000');
    SELECT ROW_COUNT();

    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('6042e441-e8e7-49e6-bbb5-29b01941e1cd', '58a22e31-200d-45ec-a8f4-da4353bea460', '2026-03-11 12:00:00.000000');
    SELECT ROW_COUNT();

    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('76ff5503-6912-4d06-b7cc-dd529ea784c5', '89887515-fd78-41bd-b273-708092b5d43e', '2026-03-13 12:00:00.000000');
    SELECT ROW_COUNT();

    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('915c6f95-df5a-444d-87ea-858dcb022e16', '3fe4e948-68e7-4d23-9b3b-d00c51d3417c', '2026-03-15 12:00:00.000000');
    SELECT ROW_COUNT();

    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('915c6f95-df5a-444d-87ea-858dcb022e16', '52f79c98-c343-491f-bc36-c2f8b0712f33', '2026-03-15 12:00:00.000000');
    SELECT ROW_COUNT();

    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('a7722912-d4c0-4745-8523-c97a9da545a3', '99606127-b8f4-4108-84e5-11a5c15085b9', '2026-03-14 12:00:00.000000');
    SELECT ROW_COUNT();

    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('a87f5e0a-1c72-4e4f-ac99-8efaf3a072ba', '89887515-fd78-41bd-b273-708092b5d43e', '2026-03-12 12:00:00.000000');
    SELECT ROW_COUNT();

    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('a87f5e0a-1c72-4e4f-ac99-8efaf3a072ba', 'b0c88999-8890-4f78-85ff-87d8a3852a27', '2026-03-12 12:00:00.000000');
    SELECT ROW_COUNT();

    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('bd87fe44-aac4-4ef9-a3c3-f83ae8abb0de', '15b32338-5b8c-42e5-88ad-bb279b43582d', '2026-03-03 12:00:00.000000');
    SELECT ROW_COUNT();

    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('dcdb2022-d30b-4c14-bfb8-c318cbceaeff', '15b32338-5b8c-42e5-88ad-bb279b43582d', '2026-03-04 12:00:00.000000');
    SELECT ROW_COUNT();

    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('e87702a8-bc94-44d0-b729-00741e7df9ab', '15b32338-5b8c-42e5-88ad-bb279b43582d', '2026-03-10 12:00:00.000000');
    SELECT ROW_COUNT();

    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('e87702a8-bc94-44d0-b729-00741e7df9ab', '3fe4e948-68e7-4d23-9b3b-d00c51d3417c', '2026-03-10 12:00:00.000000');
    SELECT ROW_COUNT();

    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('ed217a03-1db7-4203-a3d7-5bca094387d5', '52f79c98-c343-491f-bc36-c2f8b0712f33', '2026-03-05 12:00:00.000000');
    SELECT ROW_COUNT();

    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('ed217a03-1db7-4203-a3d7-5bca094387d5', '58a22e31-200d-45ec-a8f4-da4353bea460', '2026-03-05 12:00:00.000000');
    SELECT ROW_COUNT();

    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('f124152f-16aa-4863-adb6-874eb60bdfb2', '15b32338-5b8c-42e5-88ad-bb279b43582d', '2026-03-01 12:00:00.000000');
    SELECT ROW_COUNT();

    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('f124152f-16aa-4863-adb6-874eb60bdfb2', '58a22e31-200d-45ec-a8f4-da4353bea460', '2026-03-01 12:00:00.000000');
    SELECT ROW_COUNT();

    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('fef59306-fa9f-451f-b694-69a958677922', '3fe4e948-68e7-4d23-9b3b-d00c51d3417c', '2026-03-09 12:00:00.000000');
    SELECT ROW_COUNT();

    INSERT INTO `ContentGenres` (`ContentId`, `GenreId`, `AssignedAt`)
    VALUES ('fef59306-fa9f-451f-b694-69a958677922', '58a22e31-200d-45ec-a8f4-da4353bea460', '2026-03-09 12:00:00.000000');
    SELECT ROW_COUNT();

END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261030003603_InitialCreate')
BEGIN
    CREATE INDEX `IX_ContentGenres_GenreId` ON `ContentGenres` (`GenreId`);
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261030003603_InitialCreate')
BEGIN
    CREATE INDEX `IX_Contents_CreatedAt_Id` ON `Contents` (`CreatedAt`, `Id`);
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261030003603_InitialCreate')
BEGIN
    CREATE INDEX `IX_Contents_Title` ON `Contents` (`Title`);
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261030003603_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX `IX_Genres_Name` ON `Genres` (`Name`);
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261030003603_InitialCreate')
BEGIN
    CREATE INDEX `IX_Ratings_ContentId` ON `Ratings` (`ContentId`);
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261030003603_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX `IX_Ratings_UserId_ContentId` ON `Ratings` (`UserId`, `ContentId`);
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261030003603_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX `IX_UserConfigurations_UserId` ON `UserConfigurations` (`UserId`);
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261030003603_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX `IX_Users_Email` ON `Users` (`Email`);
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261030003603_InitialCreate')
BEGIN
    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20261030003603_InitialCreate', '10.0.12');
END;

COMMIT;

