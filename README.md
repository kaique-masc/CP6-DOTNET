# Recommenda API

API REST em .NET 10 para uma plataforma de recomendação de filmes, séries, documentários e animes. O projeto foi construído em etapas (CP1 a CP6) seguindo Clean Architecture, com persistência relacional em **MySQL** e o histórico de recomendações em **MongoDB**.

## Integrantes

| Nome | RM |
|------|----|
| Kaique Mascarenhas dos Santos | RM565802 |
| Felipe Augusto Ferreira Lopes | RM563982 |


## Domínio

O Recommenda guarda um catálogo de conteúdos classificados por gênero. Usuários se cadastram, avaliam os conteúdos que assistiram (nota de 1 a 5) e definem preferências de recomendação. Cada recomendação feita para um usuário (a busca e os títulos sugeridos) fica registrada em um histórico no MongoDB.

## Tecnologias

| Item | Escolha |
|------|---------|
| Plataforma | .NET 10 / ASP.NET Core Web API |
| SGBD relacional | MySQL 8 (provider `MySql.EntityFrameworkCore`) |
| Banco de documentos | MongoDB (`MongoDB.Driver` 3.x) |
| ORM | Entity Framework Core 10 (Fluent API + migrations) |
| Documentação | Swashbuckle (Swagger/OpenAPI) com XML comments |
| Versionamento | `Asp.Versioning.Mvc` + `Asp.Versioning.Mvc.ApiExplorer` |
| Autenticação | JWT Bearer (HMAC-SHA256) |
| Testes | xUnit, Moq, coverlet |

## Arquitetura

```
Recommenda.sln
├── src
│   ├── Recommenda.Domain          → entidades, enums, regras de negócio, exceções de domínio, hash de senha
│   ├── Recommenda.Application     → DTOs, serviços de aplicação, contratos (IRepository<T>, IUnitOfWork, ITokenService...)
│   ├── Recommenda.Infrastructure  → RecommendaContext (EF Core), mapeamentos, migrations, repositórios, MongoDB
│   └── Recommenda.Api             → controllers, Program.cs, JWT, Swagger, health checks, rate limit, GlobalExceptionHandler
├── tests
│   ├── Recommenda.Domain.Tests       → referencia apenas o Domain
│   └── Recommenda.Application.Tests  → referencia apenas o Application (repositórios mockados)
└── docs
    ├── mer.png / mer.pdf          → diagrama entidade-relacionamento
    ├── schema.sql                 → script gerado a partir das migrations
    ├── evidencias/                → saídas reais das chamadas, health, logs e testes
    └── img/                       → prints do Swagger
```

Dependências entre projetos:

```
Api ──► Application ──► Domain
 │           ▲
 └──► Infrastructure
```

- O **Domain** não depende de nenhum outro projeto.
- A **Application** define as interfaces de acesso a dados e não conhece EF Core nem o driver do MongoDB.
- A **Infrastructure** implementa essas interfaces.
- A **Api** só referencia a Infrastructure para fazer a composição no `Program.cs`. Nenhum controller recebe `DbContext` ou `IMongoCollection`.

## Modelo Entidade-Relacionamento

![MER](docs/mer.png)

Também disponível em [docs/mer.pdf](docs/mer.pdf).

### Entidades

Todas as tabelas usam `Id` do tipo **GUID** (`char(36)`) gerado pela aplicação, além de `CreatedAt` (obrigatório) e `UpdatedAt` (opcional), herdados de `BaseEntity`.

| Entidade | Atributos principais | Observações |
|----------|----------------------|-------------|
| **User** | Name, Email (único), Password (hash PBKDF2), BirthDate, Role | Idade mínima de 13 anos; senha com 8+ caracteres, letras e números; papel `User` ou `Admin` |
| **UserConfiguration** | UserId (único), PreferredLanguage, AllowAdultContent, ReceiveNotifications, MaxRecommendations | MaxRecommendations entre 1 e 50 |
| **Content** | Title, Synopsis?, Type, ReleaseYear, DurationMinutes, AgeRating | Type: Movie, Series, Documentary, Anime; classificação 0/10/12/14/16/18 |
| **Genre** | Name (único), Description? | Não pode ser removido enquanto houver conteúdo usando |
| **ContentGenre** | ContentId + GenreId (PK composta), AssignedAt | Tabela associativa do N:N |
| **Rating** | UserId, ContentId, Score, Comment? | Nota de 1 a 5; índice único (UserId, ContentId) |
| **RecommendationLog** *(MongoDB)* | _id, userId, query, titles[], createdAt | Coleção `recommendationLogs`, fora do EF Core |

### Relacionamentos

| Relacionamento | Cardinalidade | Opcionalidade | Exclusão |
|----------------|---------------|---------------|----------|
| User — UserConfiguration | 1 : 0..1 | Usuário pode não ter configuração; configuração sempre tem usuário | Cascade |
| User — Rating | 1 : 0..N | Usuário pode não ter avaliações; avaliação sempre tem usuário | Cascade |
| Content — Rating | 1 : 0..N | Conteúdo pode não ter avaliações; avaliação sempre tem conteúdo | Cascade |
| Content — Genre (via ContentGenre) | N : N | Conteúdo precisa de pelo menos 1 gênero (regra de domínio); gênero pode não ter conteúdos | Cascade no lado do conteúdo, Restrict no lado do gênero |
| User — RecommendationLog | 1 : 0..N (lógico) | `userId` no documento, sem FK entre bancos | — |

## Como executar

### Pré-requisitos

- .NET SDK 10
- Docker (ou MySQL 8 e MongoDB instalados localmente / MongoDB Atlas)
- Ferramenta do EF Core: `dotnet tool install --global dotnet-ef`

### 1. Subir os bancos

```bash
cp .env.example .env        # defina MYSQL_ROOT_PASSWORD
docker compose up -d
```

### 2. Configurar as connection strings (user-secrets)

O `appsettings.json` versionado tem as chaves `ConnectionStrings:RecommendaMySql` e `ConnectionStrings:RecommendaMongo` **vazias**. Os valores reais ficam nos user-secrets do projeto da API:

```bash
dotnet user-secrets set "ConnectionStrings:RecommendaMySql" "Server=localhost;Port=3306;Database=recommenda;User=root;Password=SUA_SENHA;" --project src/Recommenda.Api
dotnet user-secrets set "ConnectionStrings:RecommendaMongo" "mongodb+srv://usuario:senha@cluster.mongodb.net/" --project src/Recommenda.Api
```

Para o MongoDB do `docker-compose`, use `mongodb://localhost:27017`. Também é possível usar variáveis de ambiente (`ConnectionStrings__RecommendaMySql`, `ConnectionStrings__RecommendaMongo`).

Se alguma das duas strings estiver vazia, a API não sobe e informa qual chave precisa ser configurada.

### 3. Aplicar a migration

```bash
dotnet ef database update --project src/Recommenda.Infrastructure --startup-project src/Recommenda.Api
```

A migration `InitialCreate` cria o esquema completo e insere dados iniciais: 7 gêneros, 15 conteúdos e um usuário administrador.

### 4. Rodar a API

```bash
dotnet run --project src/Recommenda.Api --launch-profile http
```

| Recurso | URL |
|---------|-----|
| Swagger UI | http://localhost:5080/swagger |
| Health check | http://localhost:5080/health |
| Listagem v1 (deprecada) | http://localhost:5080/api/content?api-version=1.0 |
| Listagem v2 (paginada) | http://localhost:5080/api/content |
| Login | `POST` http://localhost:5080/api/v2/auth/login |

O Swagger é habilitado apenas em `Development` (perfil padrão do `launchSettings.json`).

## Autenticação (JWT)

### Configuração

Os valores do token são lidos da seção `Jwt` do `appsettings.json`:

```json
"Jwt": {
  "Issuer": "Recommenda.Api",
  "Audience": "Recommenda.Clients",
  "Key": "recommenda-chave-de-aula-somente-para-desenvolvimento-2026",
  "ExpirationMinutes": 120
}
```

A `Key` versionada é uma chave de aula, usada apenas em desenvolvimento. Em outro ambiente ela deve ser sobrescrita por user-secret ou variável de ambiente (`Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience`).

O token é assinado com HMAC-SHA256, expira em 120 minutos e traz as claims `sub` (id do usuário), `email`, `name`, `role` e `jti`. A senha **não** vai no token: no login ela é conferida por `User.VerifyPassword`, que compara com o hash PBKDF2 gravado na coluna `Password`.

A validação usa `ValidateIssuer`, `ValidateAudience`, `ValidateLifetime` e `ValidateIssuerSigningKey` = `true`, `MapInboundClaims = false` e `RoleClaimType = "role"`. Respostas 401 e 403 voltam como Problem Details.

### Usuários para teste

| Usuário | Senha | Papel | Origem |
|---------|-------|-------|--------|
| `admin@recommenda.com` | `admin1234` | Admin | seed da migration |
| criado via `POST /api/user` | escolhida no cadastro | User | todo cadastro público recebe `User` |

### Fluxo sugerido para a correção

```http
POST /api/user
Content-Type: application/json

{ "name": "Ana Souza", "email": "ana@example.com", "password": "senha1234", "birthDate": "2000-05-10" }
```

```http
POST /api/v2/auth/login
Content-Type: application/json

{ "email": "ana@example.com", "password": "senha1234" }
```

```http
GET /api/v2/auth/me
Authorization: Bearer {accessToken}
```

| Cenário | Chamada | Resultado |
|---------|---------|-----------|
| Login com senha errada ou e-mail inexistente | `POST /api/v2/auth/login` | 401 com a mesma mensagem ("E-mail ou senha inválidos.") |
| Sem token | `GET /api/v2/auth/me` ou `POST /api/content` | 401 |
| Token válido | `GET /api/v2/auth/me` | 200 com as claims |
| Token de papel `User` em rota de `Admin` | `POST /api/genre` | **403** |
| Token de papel `Admin` | `POST /api/genre` | 201 |

No Swagger, clique em **Authorize** e cole apenas o `accessToken` (o prefixo `Bearer` é adicionado automaticamente). O cadeado aparece só nas operações com `[Authorize]`.

### Endpoints protegidos

| Endpoint | Exigência |
|----------|-----------|
| `POST /api/user`, `POST /api/v2/auth/login`, `GET /health`, GETs públicos do catálogo | anônimo |
| `PUT /api/user/{id}`, `PUT /api/user/{id}/configuration` | token |
| `POST`/`PUT /api/content` | token |
| `POST`/`PUT`/`DELETE /api/rating` | token |
| `/api/v2/recommendation-logs` (todas) | token |
| `GET /api/v2/auth/me` | token |
| `POST`/`PUT`/`DELETE /api/genre`, `DELETE /api/content/{id}`, `DELETE /api/user/{id}` | role **Admin** |

## Endpoints

| Recurso | Método e rota | Versão |
|---------|---------------|--------|
| Auth | `POST /api/v2/auth/login`, `GET /api/v2/auth/me` | 2.0 |
| Content | `GET /api/content` (lista ou página), `GET /api/content/{id}`, `POST /api/content`, `PUT /api/content/{id}`, `DELETE /api/content/{id}` | 1.0 (deprecada) e 2.0 |
| Genre | `GET /api/genre`, `GET /api/genre/{id}`, `POST`, `PUT /{id}`, `DELETE /{id}` | neutra |
| Rating | `GET /api/rating/{id}`, `GET /api/rating/content/{contentId}`, `POST`, `PUT /{id}`, `DELETE /{id}` | neutra |
| User | `GET /api/user`, `GET /api/user/{id}`, `POST`, `PUT /{id}`, `DELETE /{id}`, `GET`/`PUT /api/user/{id}/configuration` | neutra |
| RecommendationLog | `POST /api/v2/recommendation-logs`, `GET /{id}`, `GET /user/{userId}`, `PUT /{id}`, `DELETE /{id}` | 2.0 |

## Versionamento (recurso `/api/content`)

- `DefaultApiVersion = 2.0` e `AssumeDefaultVersionWhenUnspecified = true`: requisição sem versão cai na 2.0.
- `ReportApiVersions = true`: as respostas trazem `api-supported-versions: 2.0` e `api-deprecated-versions: 1.0`.
- Leitores combinados: query string `api-version`, header `X-Api-Version` e segmento de URL (`/api/v1/...`, `/api/v2/...`).

| Chamada | Resultado |
|---------|-----------|
| `GET /api/content?api-version=1.0` | v1 — array com todos os conteúdos (contrato antigo) |
| `GET /api/content` com header `X-Api-Version: 1.0` | v1 — o mesmo array |
| `GET /api/content` | v2 — envelope paginado |
| `GET /api/v1/content` / `GET /api/v2/content` | equivalentes, via segmento de URL |

As duas versões chamam o mesmo `IContentService` — a v1 usa `GetAllAsync` e a v2 usa `GetPagedAsync`. `GET /{id}`, `POST`, `PUT` e `DELETE` de conteúdo existem nas duas versões; a correção pode usar a chamada sem versão (2.0).

Os controllers `User`, `Genre` e `Rating` foram marcados com `[ApiVersionNeutral]`: continuam nas mesmas rotas do CP3, aceitam qualquer versão e aparecem nos dois documentos do Swagger. `Auth` e `RecommendationLogs` foram criados já na 2.0.

O Swagger gera um documento por versão (`GroupNameFormat = 'v'VVV`). O seletor no topo da UI mostra **V2** e **V1 (deprecada)**; a descrição da v1 avisa que ela está deprecada e suas operações aparecem riscadas.

## Paginação (listagem v2)

| Parâmetro | Padrão | Regra |
|-----------|--------|-------|
| `page` | 1 | inteiro ≥ 1 |
| `pageSize` | 20 | inteiro de 1 a 100 |

```json
{
  "page": 1,
  "pageSize": 2,
  "totalItems": 15,
  "totalPages": 8,
  "hasPrevious": false,
  "hasNext": true,
  "items": [ ... ]
}
```

- `page < 1` ou `pageSize` fora de 1–100 → **400** (Problem Details com a regra na mensagem).
- Página além do total → **200** com `items: []`.
- A validação fica em `PageRequest` (Application). O corte é feito no banco por `Repository<T>.GetPagedAsync`: `CountAsync` + `OrderBy(CreatedAt).ThenBy(Id)` + `Skip` + `Take` sobre o `IQueryable`, e só então `ToListAsync`.

## Rate limit

Middleware nativo (`Microsoft.AspNetCore.RateLimiting`) com janelas fixas particionadas por IP:

| Política | Onde | Limite | Janela |
|----------|------|--------|--------|
| `fixed-write` | `POST /api/content` e `PUT /api/content/{id}` | 10 requisições | 60 segundos |
| `fixed-read` | `GET /api/content` (listagem v2) | 60 requisições | 60 segundos |

Ao estourar, a API responde **429** com header `Retry-After` (segundos) e corpo Problem Details. Os números podem ser alterados na seção `RateLimiting` do `appsettings.json`. `GET /health` usa `DisableRateLimiting()` e continua respondendo 200 depois do estouro.

## Histórico de recomendação (MongoDB)

O MySQL continua dono de `Users`, `Contents`, `Genres`, `Ratings`, `UserConfigurations` e `ContentGenres`. O histórico fica na coleção `recommendationLogs` do database `recommenda`:

```json
{
  "_id": "6f1c0a2e-1b4a-4c3d-9e10-0a1b2c3d4e5f",
  "userId": "11111111-1111-1111-1111-111111111111",
  "query": "filme de terror curto",
  "titles": ["A Bruxa", "Hereditário"],
  "createdAt": "2026-10-19T01:00:00Z"
}
```

- `MongoClient` e `IMongoDatabase` registrados como **singleton**; `GuidSerializer(GuidRepresentation.Standard)` registrado antes da primeira gravação.
- `IRecommendationLogRepository` e os DTOs ficam na Application (sem referência ao driver). A implementação com `InsertOne`, `Find`, `UpdateOne` e `DeleteOne` fica em `Infrastructure/Mongo`.
- O `UpdateOne` altera somente `query` e `titles`; `_id`, `userId` e `createdAt` não mudam.
- A listagem por usuário é ordenada por `createdAt` decrescente e limitada a 20 documentos no próprio banco (`Sort` + `Limit`). Um índice `(userId, createdAt desc)` é criado na inicialização.
- Validações (400): `query` vazia, `titles` ausente/vazio/com item em branco, `userId` igual a `Guid.Empty`.

| Operação | HTTP | Sucesso | Ausência |
|----------|------|---------|----------|
| Criar | `POST /api/v2/recommendation-logs` | 201 com o item e o id gerado | — |
| Ler um | `GET /api/v2/recommendation-logs/{id}` | 200 | 404 |
| Listar por usuário | `GET /api/v2/recommendation-logs/user/{userId}` | 200 (array, pode ser vazio) | — |
| Atualizar | `PUT /api/v2/recommendation-logs/{id}` | 204 | 404 |
| Apagar | `DELETE /api/v2/recommendation-logs/{id}` | 204 | 404 |

Todas exigem o JWT.

## Health checks

`GET /health` (anônimo, fora do rate limit e fora do Swagger) devolve um JSON com status geral, duração e a lista de checks:

| Check | Implementação | Falha |
|-------|---------------|-------|
| `self` | processo no ar | — |
| `mysql` | `AddDbContextCheck<RecommendaContext>` | Unhealthy |
| `mongodb` | `AspNetCore.HealthChecks.MongoDb` usando o `IMongoClient` da DI | Unhealthy |
| `fiap-site` | `AspNetCore.HealthChecks.Uris` para `HealthChecks:ExternalUrl` | Degraded |

| Status | HTTP |
|--------|------|
| Healthy | 200 |
| Degraded | 200 |
| Unhealthy | 503 |

O check do site externo foi registrado como *Degraded* de propósito: se a URL cair, a API continua recebendo tráfego. Se fosse *Unhealthy*, um site de terceiros fora do ar derrubaria o `/health` inteiro. Para desligá-lo, deixe `HealthChecks:ExternalUrl` vazio. O campo `error` de cada check só é preenchido em Development.

## Logs

Logs com `ILogger<T>` e propriedades nomeadas (`{ContentId}`, `{Title}`, `{TraceId}`...). Os fluxos de escrita de conteúdo, usuário, login e histórico registram início e sucesso com o `HttpContext.TraceIdentifier`. O `GlobalExceptionHandler` registra a exceção em nível `Error` com o mesmo `TraceId`; em Development esse valor também volta em `ProblemDetails.extensions.traceId`.

## Repositório genérico

- Contrato `IRepository<T> where T : BaseEntity` na Application: `GetAllAsync`, `GetByIdAsync`, `ExistsByIdAsync`, `AnyAsync`, `FindAsync`, `GetPagedAsync`, `AddAsync`, `Update` e `Delete`.
- Implementação `Repository<T>` na Infrastructure usando `Set<T>()`, com `AsNoTracking` nas leituras de lista.
- Registro: `services.AddScoped(typeof(IRepository<>), typeof(Repository<>))`.
- `IUnitOfWork.SaveChangesAsync` concentra a gravação e converte violações de chave única/FK do MySQL em `ConflictException`.
- Repositórios específicos (`IUserRepository`, `IContentRepository`, `IRatingRepository`) estendem o genérico apenas para consultas extras (busca por e-mail, conteúdo com gêneros, avaliações por conteúdo).
- O `GenreService` usa somente o `IRepository<Genre>`; `ContentService` e `RatingService` também usam o genérico para validar gêneros, usuários e conteúdos.

## Mapeamento de exceções

| Exceção | Camada | HTTP |
|---------|--------|------|
| `ValidationException` | Application | 400 |
| `DomainException` | Domain | 400 |
| `ArgumentException` | — | 400 |
| `BadHttpRequestException` | ASP.NET Core | 400 |
| Erros de model binding (`[ApiController]`) | ASP.NET Core | 400 (`ValidationProblemDetails`) |
| `ResourceNotFoundException` | Domain | 404 |
| `KeyNotFoundException` | — | 404 |
| `ConflictException` | Domain | 409 |
| Qualquer outra | — | 500 (mensagem genérica fora de Development) |

Todas as respostas de erro usam `application/problem+json`. Stack trace nunca volta na resposta; ele fica apenas no log.

## Testes

```bash
dotnet test
```

- **Recommenda.Domain.Tests** (sem mock): regras de `User` (idade mínima, senha, e-mail, `VerifyPassword`), `Rating` (nota 1–5), `Content` (gêneros, classificação, ano) e `UserConfiguration`. Padrão AAA com `[Fact]` e `[Theory]`.
- **Recommenda.Application.Tests** (Moq): serviços com repositórios mockados. Cenários de dependência ausente verificam `Times.Never` em `AddAsync`/`SaveChangesAsync`; caminhos felizes verificam `Times.Once`. Inclui a regra de paginação (`[Theory]` com page/pageSize inválidos e `[Fact]` no intervalo válido), o login (senha certa devolve token com e-mail e role; senha errada devolve `null`) e as validações do histórico.

## Migrations

Há uma única migration (`InitialCreate`) com o esquema completo e o seed. Para regenerar o script SQL:

```bash
dotnet ef migrations script --idempotent --project src/Recommenda.Infrastructure --startup-project src/Recommenda.Api -o docs/schema.sql
```

## Evidências

| Arquivo | Conteúdo |
|---------|----------|
| [00-esquema-mysql.md](docs/evidencias/00-esquema-mysql.md) | tabelas, colunas e FKs criadas no MySQL |
| [01-versionamento.md](docs/evidencias/01-versionamento.md) | v1 por query e header, v2 sem versão, headers de versão |
| [02-paginacao.md](docs/evidencias/02-paginacao.md) | páginas 1 e 2, 400 de page/pageSize, página vazia |
| [03-autenticacao-jwt.md](docs/evidencias/03-autenticacao-jwt.md) | cadastro, login 401/200, `/me` 401/200, 403 por role |
| [04-crud-e-problem-details.md](docs/evidencias/04-crud-e-problem-details.md) | CRUD relacional e erros 400/404/409 |
| [05-mongodb-recommendation-logs.md](docs/evidencias/05-mongodb-recommendation-logs.md) | ciclo completo do histórico no MongoDB |
| [06-rate-limit.md](docs/evidencias/06-rate-limit.md) | 429 com `Retry-After` e `/health` 200 em seguida |
| [07-health.md](docs/evidencias/07-health.md) | `/health` Healthy, Degraded e Unhealthy (MySQL e MongoDB parados) |
| [08-logs.md](docs/evidencias/08-logs.md) | logs com traceId |
| [09-dotnet-test.md](docs/evidencias/09-dotnet-test.md) | saída do `dotnet test` |
| [img/](docs/img) | Swagger v2, v1 deprecada e botão Authorize |
