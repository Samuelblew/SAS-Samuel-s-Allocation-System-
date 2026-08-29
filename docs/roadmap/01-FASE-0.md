# Sprint 01 — Fase 0 (fundação)

## Objetivo

Fechar a fundação local do backend para suportar identidade mínima, deploy confiável e regras pendentes da Camada 1.

## Itens

| # | Item | Critério de aceite |
|---|------|-------------------|
| 1 | Entidade `Tenant` | Tabela `tenants`, CRUD bootstrap sem header tenant |
| 2 | Entidade `User` | Tabela `users` por tenant, CRUD com `X-Tenant-Id` |
| 3 | Auth mock JWT | `POST /auth/dev-token` + Bearer; fallback header |
| 4 | Health + DB | `/health` retorna status do MySQL/InMemory |
| 5 | RN-003 | Status `AllocationNeed` atualiza ao criar/editar/remover alocação |
| 6 | CI | GitHub Actions: `dotnet build` + `dotnet test` |

## Rotas novas

```
POST   /api/v1/tenants              (sem tenant)
GET    /api/v1/tenants/{id}         (sem tenant)
POST   /api/v1/auth/dev-token       (sem tenant, só Dev/Testing)
GET    /api/v1/users
POST   /api/v1/users
GET    /api/v1/users/{id}
PUT    /api/v1/users/{id}
DELETE /api/v1/users/{id}
GET    /api/v1/health               (inclui database)
```

## Config dev

```json
"Auth": {
  "Jwt": {
    "Secret": "...mínimo 32 caracteres...",
    "Issuer": "IAS.Dev",
    "Audience": "IAS.Api",
    "ExpirationHours": 8
  }
}
```
