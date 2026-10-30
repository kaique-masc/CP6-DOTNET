# Health check — GET /health

Checks registrados: `self`, `mysql` (AddDbContextCheck), `mongodb` (AspNetCore.HealthChecks.MongoDb) e `fiap-site` (URL externa, opcional, falha como *Degraded*).

## 1. Tudo no ar (Healthy → 200)

Execução com `HealthChecks__ExternalUrl` vazio (check externo desligado).

```http
GET /health

HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8

{
  "status": "Healthy",
  "totalDuration": "00:00:00.0182408",
  "timestamp": "2026-10-30T00:37:21.373337Z",
  "checks": [
    {
      "name": "self",
      "status": "Healthy",
      "duration": "00:00:00.0000603",
      "description": "API em execução",
      "tags": [
        "live"
      ],
      "error": null
    },
    {
      "name": "mysql",
      "status": "Healthy",
      "duration": "00:00:00.0164547",
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
      "duration": "00:00:00.0027063",
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

## 2. Site externo inacessível (Degraded → 200)

O check `fiap-site` está configurado com `failureStatus: Degraded`. Para simular a queda, a URL foi trocada por um endereço que não responde (`HealthChecks__ExternalUrl=http://localhost:9/`). O relatório fica *Degraded*, mas a API continua recebendo tráfego (HTTP 200). Se esse check fosse *Unhealthy*, a queda de um site de terceiros derrubaria o `/health` inteiro para 503.

```http
GET /health

HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8

{
  "status": "Degraded",
  "totalDuration": "00:00:00.0205637",
  "timestamp": "2026-10-30T00:38:06.3687918Z",
  "checks": [
    {
      "name": "self",
      "status": "Healthy",
      "duration": "00:00:00.0000607",
      "description": "API em execução",
      "tags": [
        "live"
      ],
      "error": null
    },
    {
      "name": "mysql",
      "status": "Healthy",
      "duration": "00:00:00.0188358",
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
      "duration": "00:00:00.0032552",
      "description": null,
      "tags": [
        "db",
        "mongodb"
      ],
      "error": null
    },
    {
      "name": "fiap-site",
      "status": "Degraded",
      "duration": "00:00:00.0047167",
      "description": null,
      "tags": [
        "external"
      ],
      "error": "Connection refused (localhost:9)"
    }
  ]
}
```

## 3. MySQL parado (Unhealthy → 503)

```bash
docker stop recommenda-mysql
```

```http
GET /health

HTTP/1.1 503 Service Unavailable
Content-Type: application/json; charset=utf-8

{
  "status": "Unhealthy",
  "totalDuration": "00:00:00.0398781",
  "timestamp": "2026-10-30T00:38:14.149942Z",
  "checks": [
    {
      "name": "self",
      "status": "Healthy",
      "duration": "00:00:00.0000674",
      "description": "API em execução",
      "tags": [
        "live"
      ],
      "error": null
    },
    {
      "name": "mysql",
      "status": "Unhealthy",
      "duration": "00:00:00.0350366",
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
      "duration": "00:00:00.0025267",
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

O item `mongodb` continua *Healthy* — cada check é avaliado separadamente, mas o status geral assume o pior resultado. Após `docker start recommenda-mysql` o endpoint volta a responder 200.

## 4. MongoDB parado (Unhealthy → 503)

```bash
docker start recommenda-mysql && docker stop recommenda-mongo
```

```http
GET /health

HTTP/1.1 503 Service Unavailable
Content-Type: application/json; charset=utf-8

{
  "status": "Unhealthy",
  "totalDuration": "00:00:05.0101587",
  "timestamp": "2026-10-30T00:38:32.5157449Z",
  "checks": [
    {
      "name": "self",
      "status": "Healthy",
      "duration": "00:00:00.0000178",
      "description": "API em execução",
      "tags": [
        "live"
      ],
      "error": null
    },
    {
      "name": "mysql",
      "status": "Healthy",
      "duration": "00:00:00.1171958",
      "description": null,
      "tags": [
        "db",
        "mysql"
      ],
      "error": null
    },
    {
      "name": "mongodb",
      "status": "Unhealthy",
      "duration": "00:00:05.0061937",
      "description": null,
      "tags": [
        "db",
        "mongodb"
      ],
      "error": "The operation was canceled."
    }
  ]
}
```
