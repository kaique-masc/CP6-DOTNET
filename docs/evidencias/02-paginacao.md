# Paginação da listagem v2

### Página 1 com pageSize=2

```http
GET /api/content?page=1&pageSize=2
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

### Página 2 com pageSize=2 (não se sobrepõe à página 1)

```http
GET /api/content?page=2&pageSize=2
```

Resposta:

```http
HTTP/1.1 200
Content-Type: application/json; charset=utf-8
api-supported-versions: 2.0
api-deprecated-versions: 1.0

{
  "page": 2,
  "pageSize": 2,
  "totalItems": 15,
  "totalPages": 8,
  "hasPrevious": true,
  "hasNext": true,
  "items": [
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
    {
      "id": "dcdb2022-d30b-4c14-bfb8-c318cbceaeff",
      "title": "Corra!",
      "synopsis": "Um jovem visita a família da namorada e percebe que algo está muito errado.",
      "type": "Movie",
      "releaseYear": 2017,
      "durationMinutes": 104,
      "ageRating": 14,
      "createdAt": "2026-03-04T12:00:00Z"
    }
  ]
}
```

### page=0 → 400

```http
GET /api/content?page=0
```

Resposta:

```http
HTTP/1.1 400
Content-Type: application/problem+json
api-supported-versions: 2.0
api-deprecated-versions: 1.0

{
  "type": "https://httpstatuses.io/400",
  "title": "Requisição inválida",
  "status": 400,
  "detail": "O parâmetro 'page' deve ser maior ou igual a 1. Valor recebido: 0.",
  "instance": "/api/content",
  "traceId": "0HNPM3NKRISE9:00000001"
}
```

### pageSize=9999 → 400

```http
GET /api/content?pageSize=9999
```

Resposta:

```http
HTTP/1.1 400
Content-Type: application/problem+json
api-supported-versions: 2.0
api-deprecated-versions: 1.0

{
  "type": "https://httpstatuses.io/400",
  "title": "Requisição inválida",
  "status": 400,
  "detail": "O parâmetro 'pageSize' deve estar entre 1 e 100. Valor recebido: 9999.",
  "instance": "/api/content",
  "traceId": "0HNPM3NKRISEA:00000001"
}
```

### Página além do total → 200 com items vazio

```http
GET /api/content?page=999&pageSize=20
```

Resposta:

```http
HTTP/1.1 200
Content-Type: application/json; charset=utf-8
api-supported-versions: 2.0
api-deprecated-versions: 1.0

{
  "page": 999,
  "pageSize": 20,
  "totalItems": 15,
  "totalPages": 1,
  "hasPrevious": true,
  "hasNext": false,
  "items": []
}
```
