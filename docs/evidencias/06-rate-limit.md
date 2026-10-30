# Rate limit (fixed window) no POST /api/content

Política `fixed-write`: 10 requisições por janela de 60 s, por IP. Sequência de status ao disparar `POST /api/content` (com token) em loop:

```
201 201 201 201 201 201 201 201 201 201 429
```

### Requisição seguinte → 429 com Retry-After

```http
POST /api/content
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
Content-Type: application/json

{
  "title": "Mais um",
  "type": "Movie",
  "releaseYear": 2020,
  "durationMinutes": 90,
  "ageRating": 12,
  "genreIds": [
    "b0c88999-8890-4f78-85ff-87d8a3852a27"
  ]
}
```

Resposta:

```http
HTTP/1.1 429
Content-Type: application/problem+json
Retry-After: 60

{
  "title": "Muitas requisições",
  "status": 429,
  "detail": "Limite de requisições atingido. Tente novamente em 60 segundos.",
  "instance": "/api/content",
  "retryAfterSeconds": 60,
  "traceId": "0HNPM3R2P9EO8:00000001"
}
```

### GET /health logo após o estouro (não divide o teto)

```http
GET /health
```

```http
HTTP/1.1 200
Content-Type: application/json; charset=utf-8

{
  "status": "Healthy",
  "totalDuration": "00:00:00.0051097",
  "timestamp": "2026-10-30T00:43:12.1721098Z",
  "checks": [
    {
      "name": "self",
      "status": "Healthy",
      "duration": "00:00:00.0000545",
      "description": "API em execução",
      "tags": [
        "live"
      ],
      "error": null
    },
    {
      "name": "mysql",
      "status": "Healthy",
      "duration": "00:00:00.0042772",
      "description": null,
      "tags": [
        "db",
        "mysql"
      ],
      "error": null
    },
    {
      "name": "mongodb",
      "status": "Healthy",
      "duration": "00:00:00.0021677",
      "description": null,
      "tags": [
        "db",
        "mongodb"
      ],
      "error": null
    }
  ]
}
```

### Listagem v2 ainda disponível (política `fixed-read` separada)

```http
GET /api/content?pageSize=1
```

```http
HTTP/1.1 200
```
