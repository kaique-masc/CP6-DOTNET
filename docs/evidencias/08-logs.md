# Logs estruturados com traceId

Saída do console (`AddSimpleConsole`) durante um login, dois POST em `/api/content` (um com sucesso e um com gênero inexistente), um POST em `/api/v2/recommendation-logs` e um login recusado.

Os valores entre chaves no código (`{Title}`, `{ContentId}`, `{TraceId}`...) são propriedades nomeadas do `ILogger`, não concatenação de string. O mesmo `TraceId` (`HttpContext.TraceIdentifier`) aparece no início e no fim do fluxo de escrita e no log de erro do `GlobalExceptionHandler`, permitindo correlacionar com o `traceId` devolvido no ProblemDetails em Development.

```text
2026-10-30 00:38:50 info: Recommenda.Api.Controllers.AuthController[0] Login realizado para ana@example.com com papel User. TraceId: 0HNPM3OL3ED8H:00000001
2026-10-30 00:38:51 info: Recommenda.Api.Controllers.ContentController[0] Iniciando cadastro de conteúdo Bacurau (Movie) com 2 gênero(s). TraceId: 0HNPM3OL3ED8I:00000001
2026-10-30 00:38:51 info: Recommenda.Api.Controllers.ContentController[0] Conteúdo 0d101a5b-ed6c-4f5d-9f6c-098a47e810ff cadastrado com sucesso. TraceId: 0HNPM3OL3ED8I:00000001
2026-10-30 00:38:51 info: Recommenda.Api.Controllers.ContentController[0] Iniciando cadastro de conteúdo Teste (Movie) com 1 gênero(s). TraceId: 0HNPM3OL3ED8J:00000001
2026-10-30 00:38:51 fail: Recommenda.Api.Exceptions.GlobalExceptionHandler[0] Erro ao processar POST /api/content. Status: 404. TraceId: 0HNPM3OL3ED8J:00000001 Recommenda.Domain.Exceptions.ResourceNotFoundException: Gênero com identificador '7b0f0a43-2b4e-4b8a-9d0e-3c1f8a9e2d11' não foi encontrado.
   at Recommenda.Application.Services.ContentService.EnsureGenresExistAsync(IReadOnlyCollection`1 genreIds, CancellationToken cancellationToken) in src/Recommenda.Application/Services/ContentService.cs:line 93
   ...
2026-10-30 00:38:51 info: Recommenda.Api.Controllers.RecommendationLogsController[0] Gravando histórico de recomendação para o usuário 11111111-1111-1111-1111-111111111111. TraceId: 0HNPM3OL3ED8K:00000001
2026-10-30 00:38:51 info: Recommenda.Api.Controllers.RecommendationLogsController[0] Histórico 0711dcf9-2dc9-45ab-a44e-8dd052cb25bb gravado com 2 título(s). TraceId: 0HNPM3OL3ED8K:00000001
2026-10-30 00:38:51 warn: Recommenda.Api.Controllers.AuthController[0] Tentativa de login recusada para ana@example.com. TraceId: 0HNPM3OL3ED8L:00000001
```

Trecho do código (ContentController.Create):

```csharp
var traceId = HttpContext.TraceIdentifier;
_logger.LogInformation("Iniciando cadastro de conteúdo {Title} ({Type}) com {GenreCount} gênero(s). TraceId: {TraceId}",
    request.Title, request.Type, request.GenreIds.Count, traceId);

var content = await _contentService.CreateAsync(request, cancellationToken);

_logger.LogInformation("Conteúdo {ContentId} cadastrado com sucesso. TraceId: {TraceId}", content.Id, traceId);
```
