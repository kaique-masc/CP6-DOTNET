# CRUD relacional e respostas de erro (ProblemDetails)

### POST /api/content com token → 201

```http
POST /api/content
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
Content-Type: application/json

{
  "title": "O Iluminado",
  "synopsis": "Um escritor aceita cuidar de um hotel isolado durante o inverno.",
  "type": "Movie",
  "releaseYear": 1980,
  "durationMinutes": 146,
  "ageRating": 16,
  "genreIds": [
    "15b32338-5b8c-42e5-88ad-bb279b43582d",
    "58a22e31-200d-45ec-a8f4-da4353bea460"
  ]
}
```

Resposta:

```http
HTTP/1.1 201
Content-Type: application/json; charset=utf-8
Location: http://localhost:5080/api/content/1592887f-5cdb-4e66-820d-ea1a1d6021a6?version=2.0
api-supported-versions: 2.0
api-deprecated-versions: 1.0

{
  "id": "1592887f-5cdb-4e66-820d-ea1a1d6021a6",
  "title": "O Iluminado",
  "synopsis": "Um escritor aceita cuidar de um hotel isolado durante o inverno.",
  "type": "Movie",
  "releaseYear": 1980,
  "durationMinutes": 146,
  "ageRating": 16,
  "createdAt": "2026-10-30T00:37:03.5309127Z",
  "updatedAt": null,
  "genres": [
    {
      "id": "58a22e31-200d-45ec-a8f4-da4353bea460",
      "name": "Drama"
    },
    {
      "id": "15b32338-5b8c-42e5-88ad-bb279b43582d",
      "name": "Terror"
    }
  ]
}
```

### GET por id

```http
GET /api/content/1592887f-5cdb-4e66-820d-ea1a1d6021a6
```

Resposta:

```http
HTTP/1.1 200
Content-Type: application/json; charset=utf-8
api-supported-versions: 2.0
api-deprecated-versions: 1.0

{
  "id": "1592887f-5cdb-4e66-820d-ea1a1d6021a6",
  "title": "O Iluminado",
  "synopsis": "Um escritor aceita cuidar de um hotel isolado durante o inverno.",
  "type": "Movie",
  "releaseYear": 1980,
  "durationMinutes": 146,
  "ageRating": 16,
  "createdAt": "2026-10-30T00:37:03.530912Z",
  "updatedAt": null,
  "genres": [
    {
      "id": "58a22e31-200d-45ec-a8f4-da4353bea460",
      "name": "Drama"
    },
    {
      "id": "15b32338-5b8c-42e5-88ad-bb279b43582d",
      "name": "Terror"
    }
  ]
}
```

### PUT /api/content/{id} → 204

```http
PUT /api/content/1592887f-5cdb-4e66-820d-ea1a1d6021a6
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
Content-Type: application/json

{
  "title": "O Iluminado",
  "synopsis": "Versão estendida.",
  "type": "Movie",
  "releaseYear": 1980,
  "durationMinutes": 144,
  "ageRating": 16,
  "genreIds": [
    "15b32338-5b8c-42e5-88ad-bb279b43582d"
  ]
}
```

Resposta:

```http
HTTP/1.1 204
api-supported-versions: 2.0
api-deprecated-versions: 1.0

```

### GET por id inexistente → 404 (ProblemDetails)

```http
GET /api/content/00000000-0000-0000-0000-000000000001
```

Resposta:

```http
HTTP/1.1 404
Content-Type: application/problem+json
api-supported-versions: 2.0
api-deprecated-versions: 1.0

{
  "type": "https://httpstatuses.io/404",
  "title": "Recurso não encontrado",
  "status": 404,
  "detail": "Conteúdo com identificador '00000000-0000-0000-0000-000000000001' não foi encontrado.",
  "instance": "/api/content/00000000-0000-0000-0000-000000000001",
  "traceId": "0HNPM3NKRISEQ:00000001"
}
```

### POST com gênero inexistente → 404

```http
POST /api/content
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
Content-Type: application/json

{
  "title": "Teste",
  "type": "Movie",
  "releaseYear": 2020,
  "durationMinutes": 100,
  "ageRating": 12,
  "genreIds": [
    "7b0f0a43-2b4e-4b8a-9d0e-3c1f8a9e2d11"
  ]
}
```

Resposta:

```http
HTTP/1.1 404
Content-Type: application/problem+json
api-supported-versions: 2.0
api-deprecated-versions: 1.0

{
  "type": "https://httpstatuses.io/404",
  "title": "Recurso não encontrado",
  "status": 404,
  "detail": "Gênero com identificador '7b0f0a43-2b4e-4b8a-9d0e-3c1f8a9e2d11' não foi encontrado.",
  "instance": "/api/content",
  "traceId": "0HNPM3NKRISER:00000001"
}
```

### POST com regra de domínio violada (classificação inválida) → 400

```http
POST /api/content
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
Content-Type: application/json

{
  "title": "Teste",
  "type": "Movie",
  "releaseYear": 2020,
  "durationMinutes": 100,
  "ageRating": 15,
  "genreIds": [
    "15b32338-5b8c-42e5-88ad-bb279b43582d"
  ]
}
```

Resposta:

```http
HTTP/1.1 400
Content-Type: application/problem+json
api-supported-versions: 2.0
api-deprecated-versions: 1.0

{
  "type": "https://httpstatuses.io/400",
  "title": "Regra de negócio violada",
  "status": 400,
  "detail": "Classificação indicativa inválida. Valores aceitos: 0 (livre), 10, 12, 14, 16 e 18.",
  "instance": "/api/content",
  "traceId": "0HNPM3NKRISES:00000001"
}
```

### Usuário com e-mail duplicado → 409

```http
POST /api/user
Content-Type: application/json

{
  "name": "Ana Souza",
  "email": "ana@example.com",
  "password": "senha1234",
  "birthDate": "2000-05-10"
}
```

Resposta:

```http
HTTP/1.1 409
Content-Type: application/problem+json

{
  "type": "https://httpstatuses.io/409",
  "title": "Conflito",
  "status": 409,
  "detail": "Já existe um usuário cadastrado com o e-mail 'ana@example.com'.",
  "instance": "/api/user",
  "traceId": "0HNPM3NKRISET:00000001"
}
```

### Usuário com menos de 13 anos → 400

```http
POST /api/user
Content-Type: application/json

{
  "name": "João",
  "email": "joao@example.com",
  "password": "senha1234",
  "birthDate": "2018-01-01"
}
```

Resposta:

```http
HTTP/1.1 400
Content-Type: application/problem+json

{
  "type": "https://httpstatuses.io/400",
  "title": "Regra de negócio violada",
  "status": 400,
  "detail": "O usuário deve ter pelo menos 13 anos.",
  "instance": "/api/user",
  "traceId": "0HNPM3NKRISEU:00000001"
}
```

### Avaliação de conteúdo → 201

```http
POST /api/rating
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
Content-Type: application/json

{
  "userId": "f3a10428-d012-4299-b618-4314e8b66880",
  "contentId": "1592887f-5cdb-4e66-820d-ea1a1d6021a6",
  "score": 5,
  "comment": "Clássico"
}
```

Resposta:

```http
HTTP/1.1 201
Content-Type: application/json; charset=utf-8
Location: http://localhost:5080/api/rating/19097e76-12a2-4751-b493-dea472aafca9

{
  "id": "19097e76-12a2-4751-b493-dea472aafca9",
  "userId": "f3a10428-d012-4299-b618-4314e8b66880",
  "contentId": "1592887f-5cdb-4e66-820d-ea1a1d6021a6",
  "score": 5,
  "comment": "Clássico",
  "createdAt": "2026-10-30T00:37:03.8590912Z"
}
```

### Avaliação repetida → 409

```http
POST /api/rating
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
Content-Type: application/json

{
  "userId": "f3a10428-d012-4299-b618-4314e8b66880",
  "contentId": "1592887f-5cdb-4e66-820d-ea1a1d6021a6",
  "score": 4
}
```

Resposta:

```http
HTTP/1.1 409
Content-Type: application/problem+json

{
  "type": "https://httpstatuses.io/409",
  "title": "Conflito",
  "status": 409,
  "detail": "Este usuário já avaliou este conteúdo.",
  "instance": "/api/rating",
  "traceId": "0HNPM3NKRISF1:00000001"
}
```

### Nota fora de 1–5 → 400

```http
POST /api/rating
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
Content-Type: application/json

{
  "userId": "f3a10428-d012-4299-b618-4314e8b66880",
  "contentId": "f124152f-16aa-4863-adb6-874eb60bdfb2",
  "score": 9
}
```

Resposta:

```http
HTTP/1.1 400
Content-Type: application/problem+json

{
  "type": "https://httpstatuses.io/400",
  "title": "Regra de negócio violada",
  "status": 400,
  "detail": "A nota deve estar entre 1 e 5.",
  "instance": "/api/rating",
  "traceId": "0HNPM3NKRISF2:00000001"
}
```

### Remover gênero em uso → 409

```http
DELETE /api/genre/15b32338-5b8c-42e5-88ad-bb279b43582d
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
```

Resposta:

```http
HTTP/1.1 409
Content-Type: application/problem+json

{
  "type": "https://httpstatuses.io/409",
  "title": "Conflito",
  "status": 409,
  "detail": "O registro está em uso por outros dados e não pode ser removido.",
  "instance": "/api/genre/15b32338-5b8c-42e5-88ad-bb279b43582d",
  "traceId": "0HNPM3NKRISF3:00000001"
}
```

### Configuração do usuário (1:0..1) → 200

```http
PUT /api/user/f3a10428-d012-4299-b618-4314e8b66880/configuration
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
Content-Type: application/json

{
  "preferredLanguage": "pt-BR",
  "allowAdultContent": false,
  "receiveNotifications": true,
  "maxRecommendations": 15
}
```

Resposta:

```http
HTTP/1.1 200
Content-Type: application/json; charset=utf-8

{
  "id": "03f7df3c-9b05-4784-8864-339d8566a4aa",
  "userId": "f3a10428-d012-4299-b618-4314e8b66880",
  "preferredLanguage": "pt-BR",
  "allowAdultContent": false,
  "receiveNotifications": true,
  "maxRecommendations": 15,
  "updatedAt": null
}
```
