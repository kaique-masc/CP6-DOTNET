# MongoDB — histórico de recomendação (recommendationLogs)

### POST sem Authorization → 401

```http
POST /api/v2/recommendation-logs
Content-Type: application/json

{
  "userId": "11111111-1111-1111-1111-111111111111",
  "query": "filme de terror curto",
  "titles": [
    "A Bruxa",
    "Hereditário"
  ]
}
```

Resposta:

```http
HTTP/1.1 401
Content-Type: application/problem+json

{
  "title": "Não autenticado",
  "status": 401,
  "detail": "Envie um token válido no header Authorization: Bearer {token}.",
  "instance": "/api/v2/recommendation-logs",
  "traceId": "0HNPM3NKRISF5:00000001"
}
```

### POST com token → 201 (id diferente do userId)

```http
POST /api/v2/recommendation-logs
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
Content-Type: application/json

{
  "userId": "11111111-1111-1111-1111-111111111111",
  "query": "filme de terror curto",
  "titles": [
    "A Bruxa",
    "Hereditário"
  ]
}
```

Resposta:

```http
HTTP/1.1 201
Content-Type: application/json; charset=utf-8
Location: http://localhost:5080/api/v2.0/recommendation-logs/0684a580-7277-45dd-ac55-12f3ba24d7f9
api-supported-versions: 2.0

{
  "id": "0684a580-7277-45dd-ac55-12f3ba24d7f9",
  "userId": "11111111-1111-1111-1111-111111111111",
  "query": "filme de terror curto",
  "titles": [
    "A Bruxa",
    "Hereditário"
  ],
  "createdAt": "2026-10-30T00:37:04.013402Z"
}
```

### GET por id

```http
GET /api/v2/recommendation-logs/0684a580-7277-45dd-ac55-12f3ba24d7f9
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
```

Resposta:

```http
HTTP/1.1 200
Content-Type: application/json; charset=utf-8
api-supported-versions: 2.0

{
  "id": "0684a580-7277-45dd-ac55-12f3ba24d7f9",
  "userId": "11111111-1111-1111-1111-111111111111",
  "query": "filme de terror curto",
  "titles": [
    "A Bruxa",
    "Hereditário"
  ],
  "createdAt": "2026-10-30T00:37:04.013Z"
}
```

### GET por usuário (ordenado por createdAt desc, até 20)

```http
GET /api/v2/recommendation-logs/user/11111111-1111-1111-1111-111111111111
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
```

Resposta:

```http
HTTP/1.1 200
Content-Type: application/json; charset=utf-8
api-supported-versions: 2.0

[
  {
    "id": "0684a580-7277-45dd-ac55-12f3ba24d7f9",
    "userId": "11111111-1111-1111-1111-111111111111",
    "query": "filme de terror curto",
    "titles": [
      "A Bruxa",
      "Hereditário"
    ],
    "createdAt": "2026-10-30T00:37:04.013Z"
  }
]
```

### PUT → 204

```http
PUT /api/v2/recommendation-logs/0684a580-7277-45dd-ac55-12f3ba24d7f9
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
Content-Type: application/json

{
  "query": "filme de terror curto, dublado",
  "titles": [
    "A Bruxa",
    "Hereditário",
    "O Babadook"
  ]
}
```

Resposta:

```http
HTTP/1.1 204
api-supported-versions: 2.0

```

### GET após o PUT (mesmo userId e createdAt)

```http
GET /api/v2/recommendation-logs/0684a580-7277-45dd-ac55-12f3ba24d7f9
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
```

Resposta:

```http
HTTP/1.1 200
Content-Type: application/json; charset=utf-8
api-supported-versions: 2.0

{
  "id": "0684a580-7277-45dd-ac55-12f3ba24d7f9",
  "userId": "11111111-1111-1111-1111-111111111111",
  "query": "filme de terror curto, dublado",
  "titles": [
    "A Bruxa",
    "Hereditário",
    "O Babadook"
  ],
  "createdAt": "2026-10-30T00:37:04.013Z"
}
```

### DELETE → 204

```http
DELETE /api/v2/recommendation-logs/0684a580-7277-45dd-ac55-12f3ba24d7f9
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
```

Resposta:

```http
HTTP/1.1 204
api-supported-versions: 2.0

```

### GET após o DELETE → 404

```http
GET /api/v2/recommendation-logs/0684a580-7277-45dd-ac55-12f3ba24d7f9
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
```

Resposta:

```http
HTTP/1.1 404
Content-Type: application/problem+json
api-supported-versions: 2.0

{
  "type": "https://httpstatuses.io/404",
  "title": "Recurso não encontrado",
  "status": 404,
  "detail": "Histórico de recomendação com identificador '0684a580-7277-45dd-ac55-12f3ba24d7f9' não foi encontrado.",
  "instance": "/api/v2/recommendation-logs/0684a580-7277-45dd-ac55-12f3ba24d7f9",
  "traceId": "0HNPM3NKRISFC:00000001"
}
```

### Validação: query vazia → 400

```http
POST /api/v2/recommendation-logs
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
Content-Type: application/json

{
  "userId": "11111111-1111-1111-1111-111111111111",
  "query": "",
  "titles": [
    "A Bruxa"
  ]
}
```

Resposta:

```http
HTTP/1.1 400
Content-Type: application/problem+json
api-supported-versions: 2.0

{
  "type": "https://httpstatuses.io/400",
  "title": "Requisição inválida",
  "status": 400,
  "detail": "O campo 'query' é obrigatório.",
  "instance": "/api/v2/recommendation-logs",
  "traceId": "0HNPM3NKRISFD:00000001"
}
```

### Validação: titles com item em branco → 400

```http
POST /api/v2/recommendation-logs
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
Content-Type: application/json

{
  "userId": "11111111-1111-1111-1111-111111111111",
  "query": "terror",
  "titles": [
    "A Bruxa",
    " "
  ]
}
```

Resposta:

```http
HTTP/1.1 400
Content-Type: application/problem+json
api-supported-versions: 2.0

{
  "type": "https://httpstatuses.io/400",
  "title": "Requisição inválida",
  "status": 400,
  "detail": "O campo 'titles' não pode conter itens em branco.",
  "instance": "/api/v2/recommendation-logs",
  "traceId": "0HNPM3NKRISFE:00000001"
}
```

### Validação: userId vazio → 400

```http
POST /api/v2/recommendation-logs
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
Content-Type: application/json

{
  "userId": "00000000-0000-0000-0000-000000000000",
  "query": "terror",
  "titles": [
    "A Bruxa"
  ]
}
```

Resposta:

```http
HTTP/1.1 400
Content-Type: application/problem+json
api-supported-versions: 2.0

{
  "type": "https://httpstatuses.io/400",
  "title": "Requisição inválida",
  "status": 400,
  "detail": "O campo 'userId' é obrigatório e não pode ser um Guid vazio.",
  "instance": "/api/v2/recommendation-logs",
  "traceId": "0HNPM3NKRISFF:00000001"
}
```

### Listagem com userId vazio → 400

```http
GET /api/v2/recommendation-logs/user/00000000-0000-0000-0000-000000000000
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
```

Resposta:

```http
HTTP/1.1 400
Content-Type: application/problem+json
api-supported-versions: 2.0

{
  "type": "https://httpstatuses.io/400",
  "title": "Requisição inválida",
  "status": 400,
  "detail": "O campo 'userId' é obrigatório e não pode ser um Guid vazio.",
  "instance": "/api/v2/recommendation-logs/user/00000000-0000-0000-0000-000000000000",
  "traceId": "0HNPM3NKRISFG:00000001"
}
```
