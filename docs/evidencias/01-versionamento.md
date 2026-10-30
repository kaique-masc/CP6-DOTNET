# Versionamento do recurso /api/content

### v1 via query string (contrato antigo: array)

```http
GET /api/content?api-version=1.0
```

Resposta:

```http
HTTP/1.1 200
Content-Type: application/json; charset=utf-8
api-supported-versions: 2.0
api-deprecated-versions: 1.0

[
  {
    "id": "f124152f-16aa-4863-adb6-874eb60bdfb2",
    "title": "A Bruxa",
    "synopsis": "Uma família puritana isolada na Nova Inglaterra do século XVII enfrenta forças misteriosas.",
    "type": "Movie",
    "releaseYear": 2015,
    "durationMinutes": 92,
    "ageRating": 14,
    "createdAt": "2026-03-01T12:00:00Z"
  },
  {
    "id": "17e9aaf8-020b-4c62-8a25-f3a9ef958cee",
    "title": "Hereditário",
    "synopsis": "Após a morte da avó, uma família começa a descobrir segredos assustadores.",
    "type": "Movie",
    "releaseYear": 2018,
    "durationMinutes": 127,
    "ageRating": 16,
    "createdAt": "2026-03-02T12:00:00Z"
  },
  {
    "id": "bd87fe44-aac4-4ef9-a3c3-f83ae8abb0de",
    "title": "O Babadook",
    "synopsis": "Uma mãe viúva e o filho são assombrados por uma criatura saída de um livro infantil.",
    "type": "Movie",
    "releaseYear": 2014,
    "durationMinutes": 94,
    "ageRating": 14,
    "createdAt": "2026-03-03T12:00:00Z"
  },
  "... (+12 itens)"
]
```

### v1 via header X-Api-Version

```http
GET /api/content
X-Api-Version: 1.0
```

Resposta:

```http
HTTP/1.1 200
Content-Type: application/json; charset=utf-8
api-supported-versions: 2.0
api-deprecated-versions: 1.0

[
  {
    "id": "f124152f-16aa-4863-adb6-874eb60bdfb2",
    "title": "A Bruxa",
    "synopsis": "Uma família puritana isolada na Nova Inglaterra do século XVII enfrenta forças misteriosas.",
    "type": "Movie",
    "releaseYear": 2015,
    "durationMinutes": 92,
    "ageRating": 14,
    "createdAt": "2026-03-01T12:00:00Z"
  },
  {
    "id": "17e9aaf8-020b-4c62-8a25-f3a9ef958cee",
    "title": "Hereditário",
    "synopsis": "Após a morte da avó, uma família começa a descobrir segredos assustadores.",
    "type": "Movie",
    "releaseYear": 2018,
    "durationMinutes": 127,
    "ageRating": 16,
    "createdAt": "2026-03-02T12:00:00Z"
  },
  "... (+13 itens)"
]
```

### Sem versão: cai na v2 (envelope paginado)

```http
GET /api/content?pageSize=3
```

Resposta:

```http
HTTP/1.1 200
Content-Type: application/json; charset=utf-8
api-supported-versions: 2.0
api-deprecated-versions: 1.0

{
  "page": 1,
  "pageSize": 3,
  "totalItems": 15,
  "totalPages": 5,
  "hasPrevious": false,
  "hasNext": true,
  "items": [
    {
      "id": "f124152f-16aa-4863-adb6-874eb60bdfb2",
      "title": "A Bruxa",
      "synopsis": "Uma família puritana isolada na Nova Inglaterra do século XVII enfrenta forças misteriosas.",
      "type": "Movie",
      "releaseYear": 2015,
      "durationMinutes": 92,
      "ageRating": 14,
      "createdAt": "2026-03-01T12:00:00Z"
    },
    {
      "id": "17e9aaf8-020b-4c62-8a25-f3a9ef958cee",
      "title": "Hereditário",
      "synopsis": "Após a morte da avó, uma família começa a descobrir segredos assustadores.",
      "type": "Movie",
      "releaseYear": 2018,
      "durationMinutes": 127,
      "ageRating": 16,
      "createdAt": "2026-03-02T12:00:00Z"
    },
    {
      "id": "bd87fe44-aac4-4ef9-a3c3-f83ae8abb0de",
      "title": "O Babadook",
      "synopsis": "Uma mãe viúva e o filho são assombrados por uma criatura saída de um livro infantil.",
      "type": "Movie",
      "releaseYear": 2014,
      "durationMinutes": 94,
      "ageRating": 14,
      "createdAt": "2026-03-03T12:00:00Z"
    }
  ]
}
```

### v2 via segmento de URL

```http
GET /api/v2/content?pageSize=2
```

Resposta:

```http
HTTP/1.1 200
Content-Type: application/json; charset=utf-8
api-supported-versions: 2.0
api-deprecated-versions: 1.0

{
  "page": 1,
  "pageSize": 2,
  "totalItems": 15,
  "totalPages": 8,
  "hasPrevious": false,
  "hasNext": true,
  "items": [
    {
      "id": "f124152f-16aa-4863-adb6-874eb60bdfb2",
      "title": "A Bruxa",
      "synopsis": "Uma família puritana isolada na Nova Inglaterra do século XVII enfrenta forças misteriosas.",
      "type": "Movie",
      "releaseYear": 2015,
      "durationMinutes": 92,
      "ageRating": 14,
      "createdAt": "2026-03-01T12:00:00Z"
    },
    {
      "id": "17e9aaf8-020b-4c62-8a25-f3a9ef958cee",
      "title": "Hereditário",
      "synopsis": "Após a morte da avó, uma família começa a descobrir segredos assustadores.",
      "type": "Movie",
      "releaseYear": 2018,
      "durationMinutes": 127,
      "ageRating": 16,
      "createdAt": "2026-03-02T12:00:00Z"
    }
  ]
}
```
