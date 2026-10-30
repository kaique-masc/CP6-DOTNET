# Esquema físico no MySQL

Banco criado com `dotnet ef database update` a partir da migration `InitialCreate`. O script SQL completo (idempotente) está em [../schema.sql](../schema.sql).

```text
mysql> SHOW TABLES;
+-----------------------+
| Tables_in_recommenda  |
+-----------------------+
| ContentGenres         |
| Contents              |
| Genres                |
| Ratings               |
| UserConfigurations    |
| Users                 |
| __EFMigrationsHistory |
+-----------------------+

mysql> DESCRIBE Users;
+-----------+--------------+------+-----+---------+-------+
| Field     | Type         | Null | Key | Default | Extra |
+-----------+--------------+------+-----+---------+-------+
| Id        | char(36)     | NO   | PRI | NULL    |       |
| Name      | varchar(120) | NO   |     | NULL    |       |
| Email     | varchar(180) | NO   | UNI | NULL    |       |
| Password  | varchar(256) | NO   |     | NULL    |       |
| BirthDate | date         | NO   |     | NULL    |       |
| Role      | varchar(20)  | NO   |     | NULL    |       |
| CreatedAt | datetime(6)  | NO   |     | NULL    |       |
| UpdatedAt | datetime(6)  | YES  |     | NULL    |       |
+-----------+--------------+------+-----+---------+-------+

mysql> DESCRIBE UserConfigurations;
+----------------------+-------------+------+-----+---------+-------+
| Field                | Type        | Null | Key | Default | Extra |
+----------------------+-------------+------+-----+---------+-------+
| Id                   | char(36)    | NO   | PRI | NULL    |       |
| UserId               | char(36)    | NO   | UNI | NULL    |       |
| PreferredLanguage    | varchar(10) | NO   |     | NULL    |       |
| AllowAdultContent    | tinyint(1)  | NO   |     | NULL    |       |
| ReceiveNotifications | tinyint(1)  | NO   |     | NULL    |       |
| MaxRecommendations   | int         | NO   |     | NULL    |       |
| CreatedAt            | datetime(6) | NO   |     | NULL    |       |
| UpdatedAt            | datetime(6) | YES  |     | NULL    |       |
+----------------------+-------------+------+-----+---------+-------+

mysql> DESCRIBE Contents;
+-----------------+---------------+------+-----+---------+-------+
| Field           | Type          | Null | Key | Default | Extra |
+-----------------+---------------+------+-----+---------+-------+
| Id              | char(36)      | NO   | PRI | NULL    |       |
| Title           | varchar(200)  | NO   | MUL | NULL    |       |
| Synopsis        | varchar(2000) | YES  |     | NULL    |       |
| Type            | varchar(20)   | NO   |     | NULL    |       |
| ReleaseYear     | int           | NO   |     | NULL    |       |
| DurationMinutes | int           | NO   |     | NULL    |       |
| AgeRating       | int           | NO   |     | NULL    |       |
| CreatedAt       | datetime(6)   | NO   | MUL | NULL    |       |
| UpdatedAt       | datetime(6)   | YES  |     | NULL    |       |
+-----------------+---------------+------+-----+---------+-------+

mysql> DESCRIBE Genres;
+-------------+--------------+------+-----+---------+-------+
| Field       | Type         | Null | Key | Default | Extra |
+-------------+--------------+------+-----+---------+-------+
| Id          | char(36)     | NO   | PRI | NULL    |       |
| Name        | varchar(60)  | NO   | UNI | NULL    |       |
| Description | varchar(250) | YES  |     | NULL    |       |
| CreatedAt   | datetime(6)  | NO   |     | NULL    |       |
| UpdatedAt   | datetime(6)  | YES  |     | NULL    |       |
+-------------+--------------+------+-----+---------+-------+

mysql> DESCRIBE ContentGenres;
+------------+-------------+------+-----+---------+-------+
| Field      | Type        | Null | Key | Default | Extra |
+------------+-------------+------+-----+---------+-------+
| ContentId  | char(36)    | NO   | PRI | NULL    |       |
| GenreId    | char(36)    | NO   | PRI | NULL    |       |
| AssignedAt | datetime(6) | NO   |     | NULL    |       |
+------------+-------------+------+-----+---------+-------+

mysql> DESCRIBE Ratings;
+-----------+--------------+------+-----+---------+-------+
| Field     | Type         | Null | Key | Default | Extra |
+-----------+--------------+------+-----+---------+-------+
| Id        | char(36)     | NO   | PRI | NULL    |       |
| UserId    | char(36)     | NO   | MUL | NULL    |       |
| ContentId | char(36)     | NO   | MUL | NULL    |       |
| Score     | int          | NO   |     | NULL    |       |
| Comment   | varchar(500) | YES  |     | NULL    |       |
| CreatedAt | datetime(6)  | NO   |     | NULL    |       |
| UpdatedAt | datetime(6)  | YES  |     | NULL    |       |
+-----------+--------------+------+-----+---------+-------+

mysql> SELECT TABLE_NAME, CONSTRAINT_NAME, REFERENCED_TABLE_NAME, DELETE_RULE FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA = 'recommenda';
+--------------------+-------------------------------------+-----------------------+-------------+
| TABLE_NAME         | CONSTRAINT_NAME                     | REFERENCED_TABLE_NAME | DELETE_RULE |
+--------------------+-------------------------------------+-----------------------+-------------+
| ContentGenres      | FK_ContentGenres_Contents_ContentId | Contents              | CASCADE     |
| ContentGenres      | FK_ContentGenres_Genres_GenreId     | Genres                | RESTRICT    |
| Ratings            | FK_Ratings_Contents_ContentId       | Contents              | CASCADE     |
| Ratings            | FK_Ratings_Users_UserId             | Users                 | CASCADE     |
| UserConfigurations | FK_UserConfigurations_Users_UserId  | Users                 | CASCADE     |
+--------------------+-------------------------------------+-----------------------+-------------+

mysql> SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory;
+------------------------------+----------------+
| MigrationId                  | ProductVersion |
+------------------------------+----------------+
| 20261030003603_InitialCreate | 10.0.12        |
+------------------------------+----------------+
```
