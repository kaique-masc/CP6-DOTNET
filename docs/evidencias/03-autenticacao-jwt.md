# Autenticação JWT

### Cadastro de usuário (anônimo)

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
HTTP/1.1 201
Content-Type: application/json; charset=utf-8
Location: http://localhost:5080/api/user/f3a10428-d012-4299-b618-4314e8b66880

{
  "id": "f3a10428-d012-4299-b618-4314e8b66880",
  "name": "Ana Souza",
  "email": "ana@example.com",
  "birthDate": "2000-05-10",
  "age": 26,
  "role": "User",
  "createdAt": "2026-10-30T00:37:02.9546394Z"
}
```

### Login com senha errada → 401

```http
POST /api/v2/auth/login
Content-Type: application/json

{
  "email": "ana@example.com",
  "password": "senhaerrada1"
}
```

Resposta:

```http
HTTP/1.1 401
Content-Type: application/problem+json; charset=utf-8
api-supported-versions: 2.0

{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.2",
  "title": "Credenciais inválidas",
  "status": 401,
  "detail": "E-mail ou senha inválidos.",
  "traceId": "00-c6132659616d716c16c30e938cc739a2-909cfd608ac70b1c-00"
}
```

### Login com e-mail inexistente → 401 (mesma mensagem)

```http
POST /api/v2/auth/login
Content-Type: application/json

{
  "email": "ninguem@example.com",
  "password": "senha1234"
}
```

Resposta:

```http
HTTP/1.1 401
Content-Type: application/problem+json; charset=utf-8
api-supported-versions: 2.0

{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.2",
  "title": "Credenciais inválidas",
  "status": 401,
  "detail": "E-mail ou senha inválidos.",
  "traceId": "00-529110b5fd98ea360753ee3626485d4b-c5ed867cf2a95bf5-00"
}
```

### Login correto → 200

```http
POST /api/v2/auth/login
Content-Type: application/json

{
  "email": "ana@example.com",
  "password": "senha1234"
}
```

Resposta:

```http
HTTP/1.1 200
Content-Type: application/json; charset=utf-8
api-supported-versions: 2.0

{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJSZWNvbW1lbmRhLkNsaWVudHMiLCJpc3MiOiJSZWNvbW1lbmRhLkFwaSIsImV4cCI6MTc5MzMyNzgyMywiaWF0IjoxNzkzMzIwNjIzLCJuYmYiOjE3OTMzMjA2MjMsInN1YiI6ImYzYTEwNDI4LWQwMTItNDI5OS1iNjE4LTQzMTRlOGI2Njg4MCIsImVtYWlsIjoiYW5hQGV4YW1wbGUuY29tIiwibmFtZSI6IkFuYSBTb3V6YSIsInJvbGUiOiJVc2VyIiwianRpIjoiNzI5MTNmOGEtMDBkNC00MGM0LTk2MzctMDA3ZmIwNGJmNTYxIn0.8EAXl6O8Yy_w3B8H0F0M5DdIfSFe22p6z--u9WTMfsc",
  "expiration": "2026-10-30T02:37:03.320844Z",
  "email": "ana@example.com",
  "name": "Ana Souza",
  "role": "User"
}
```

### GET /me sem token → 401

```http
GET /api/v2/auth/me
```

Resposta:

```http
HTTP/1.1 401
Content-Type: application/problem+json

{
  "title": "Não autenticado",
  "status": 401,
  "detail": "Envie um token válido no header Authorization: Bearer {token}.",
  "instance": "/api/v2/auth/me",
  "traceId": "0HNPM3NKRISEG:00000001"
}
```

### GET /me com token → 200 e claims

```http
GET /api/v2/auth/me
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
```

Resposta:

```http
HTTP/1.1 200
Content-Type: application/json; charset=utf-8
api-supported-versions: 2.0

{
  "claims": [
    {
      "type": "aud",
      "value": "Recommenda.Clients"
    },
    {
      "type": "iss",
      "value": "Recommenda.Api"
    },
    {
      "type": "exp",
      "value": "1793327823"
    },
    {
      "type": "iat",
      "value": "1793320623"
    },
    {
      "type": "nbf",
      "value": "1793320623"
    },
    {
      "type": "sub",
      "value": "f3a10428-d012-4299-b618-4314e8b66880"
    },
    {
      "type": "email",
      "value": "ana@example.com"
    },
    {
      "type": "name",
      "value": "Ana Souza"
    },
    {
      "type": "role",
      "value": "User"
    },
    {
      "type": "jti",
      "value": "72913f8a-00d4-40c4-9637-007fb04bf561"
    }
  ]
}
```

### POST de conteúdo sem token → 401

```http
POST /api/content
Content-Type: application/json

{
  "title": "O Iluminado",
  "type": "Movie",
  "releaseYear": 1980,
  "durationMinutes": 146,
  "ageRating": 16,
  "genreIds": [
    "15b32338-5b8c-42e5-88ad-bb279b43582d"
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
  "instance": "/api/content",
  "traceId": "0HNPM3NKRISEI:00000001"
}
```

### POST /api/genre com token de papel User → 403

```http
POST /api/genre
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
Content-Type: application/json

{
  "name": "Suspense"
}
```

Resposta:

```http
HTTP/1.1 403
Content-Type: application/problem+json

{
  "title": "Acesso negado",
  "status": 403,
  "detail": "O token informado não possui o papel necessário para este recurso.",
  "instance": "/api/genre",
  "traceId": "0HNPM3NKRISEJ:00000001"
}
```

### Login do administrador (seed)

```http
POST /api/v2/auth/login
Content-Type: application/json

{
  "email": "admin@recommenda.com",
  "password": "admin1234"
}
```

Resposta:

```http
HTTP/1.1 200
Content-Type: application/json; charset=utf-8
api-supported-versions: 2.0

{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJSZWNvbW1lbmRhLkNsaWVudHMiLCJpc3MiOiJSZWNvbW1lbmRhLkFwaSIsImV4cCI6MTc5MzMyNzgyMywiaWF0IjoxNzkzMzIwNjIzLCJuYmYiOjE3OTMzMjA2MjMsInN1YiI6IjMwMjlhZWYwLTcxOTItNDUzZS1hMmQzLTFhZWE4ODY0MDk1NCIsImVtYWlsIjoiYWRtaW5AcmVjb21tZW5kYS5jb20iLCJuYW1lIjoiQWRtaW5pc3RyYWRvciIsInJvbGUiOiJBZG1pbiIsImp0aSI6IjUxZTFmYTIzLThkYjUtNDFmYy05YjEyLTg3NTg1OGVlMjgyNCJ9.-Utocih9NZii6IF3oetSnvqVLCIDKZ9EimxKCmWJ96U",
  "expiration": "2026-10-30T02:37:03.4328131Z",
  "email": "admin@recommenda.com",
  "name": "Administrador",
  "role": "Admin"
}
```

### POST /api/genre com token Admin → 201

```http
POST /api/genre
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...(token)
Content-Type: application/json

{
  "name": "Suspense",
  "description": "Tensão e mistério"
}
```

Resposta:

```http
HTTP/1.1 201
Content-Type: application/json; charset=utf-8
Location: http://localhost:5080/api/genre/710addca-95bc-4dd8-b11e-6cce824698c4

{
  "id": "710addca-95bc-4dd8-b11e-6cce824698c4",
  "name": "Suspense",
  "description": "Tensão e mistério",
  "createdAt": "2026-10-30T00:37:03.44378Z"
}
```
