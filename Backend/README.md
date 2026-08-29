# IAS — Backend

Solution .NET 10 (Clean Architecture) para o Intelligent Allocation System.

## Estrutura

```
Backend/
  IAS.sln           # solution principal
  src/
    IAS.Domain/         # entidades, regras de domínio (sem dependências externas)
    IAS.Application/    # MediatR, FluentValidation, casos de uso
    IAS.Infrastructure/ # EF Core, Pomelo MySQL, repositórios
    IAS.Api/              # ASP.NET Core, controllers, Swagger
```

### Referências entre camadas

```
IAS.Api → IAS.Application, IAS.Infrastructure
IAS.Infrastructure → IAS.Application, IAS.Domain
IAS.Application → IAS.Domain
```

## Pacotes

| Projeto | Pacotes |
|---------|---------|
| Application | MediatR, FluentValidation |
| Infrastructure | EF Core 10, Pomelo.EntityFrameworkCore.MySql |
| Api | MediatR, Swashbuckle (Swagger) |

> **Nota:** MySQL usa `Microting.EntityFrameworkCore.MySql` 10.x (fork compatível com EF Core 10). Pacote oficial Pomelo 10 ainda em preview.

## Comandos

```powershell
cd Backend
dotnet build
dotnet run --project src/IAS.Api
```

Swagger (dev): `https://localhost:7xxx/swagger`

Health: `GET /api/v1/health`

## Connection string (dev)

Não commitar senha. Atalho (recomendado):

```powershell
cd Backend
.\scripts\setup-dev-database.ps1
```

O script pede a senha do MySQL, cria `ias_dev`, grava User Secrets e roda `dotnet ef database update`.

Ou manualmente:

```powershell
cd src/IAS.Api
dotnet user-secrets set "ConnectionStrings:Default" "Server=localhost;Port=3306;Database=ias_dev;User=root;Password=SUA_SENHA;"
dotnet ef database update --project ../IAS.Infrastructure
```

## API Skills (`/api/v1/skills`)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/skills?page=1&pageSize=20` | Lista paginada |
| GET | `/api/v1/skills/{id}` | Detalhe |
| POST | `/api/v1/skills` | Criar |
| PUT | `/api/v1/skills/{id}` | Atualizar |
| DELETE | `/api/v1/skills/{id}` | Soft delete |

Header obrigatório (dev): `X-Tenant-Id: {guid}`

## Migration MySQL

```powershell
dotnet tool install --global dotnet-ef   # se ainda não tiver
cd src/IAS.Api
dotnet ef database update --project ../IAS.Infrastructure
```

Ou aplicar manualmente a migration `20260603120000_AddSkillsTable` no banco `ias_dev`.

## Testes

```powershell
dotnet test IAS.sln
```

## API People (`/api/v1/people`)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/people` | Lista paginada |
| GET | `/api/v1/people/{id}` | Detalhe com skills |
| POST | `/api/v1/people` | Criar pessoa |
| PUT | `/api/v1/people/{id}` | Atualizar |
| DELETE | `/api/v1/people/{id}` | Soft delete (inclui person_skills) |
| POST | `/api/v1/people/{id}/skills` | Vincular skill do catálogo |
| PUT | `/api/v1/people/{id}/skills/{personSkillId}` | Atualizar nível/notas |
| DELETE | `/api/v1/people/{id}/skills/{personSkillId}` | Remover vínculo |

Status da pessoa: `Active`, `Vacation`, `NoticePeriod`, `Offboarded`, `Contractor`  
Nível de skill: `Beginner`, `Intermediate`, `Advanced`, `Expert`

Migration: `20260603140000_AddPeopleAndPersonSkills`

## API Unavailabilities (`/api/v1/people/{personId}/unavailabilities`)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `.../unavailabilities` | Lista paginada por pessoa |
| GET | `.../unavailabilities/{id}` | Detalhe |
| POST | `.../unavailabilities` | Criar período |
| PUT | `.../unavailabilities/{id}` | Atualizar |
| DELETE | `.../unavailabilities/{id}` | Soft delete |

Tipo: `Vacation`, `SickLeave`, `Training`, `Personal`, `Other`  
Regra: períodos sobrepostos na mesma pessoa retornam **409 Conflict**.

Migration: `20260603150000_AddUnavailabilities`

## API Clients (`/api/v1/clients`)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/clients` | Lista paginada |
| GET | `/api/v1/clients/{id}` | Detalhe |
| POST | `/api/v1/clients` | Criar |
| PUT | `/api/v1/clients/{id}` | Atualizar |
| DELETE | `/api/v1/clients/{id}` | Soft delete |

## API Projects (`/api/v1/projects`)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/projects?clientId={guid}` | Lista (filtro opcional por cliente) |
| GET | `/api/v1/projects/{id}` | Detalhe |
| POST | `/api/v1/projects` | Criar (exige `clientId`) |
| PUT | `/api/v1/projects/{id}` | Atualizar |
| DELETE | `/api/v1/projects/{id}` | Soft delete |

Status: `Proposal`, `Approved`, `InProgress`, `Paused`, `Closed`  
Prioridade: `Low`, `Medium`, `High`, `Critical`

Migration: `20260603160000_AddClientsAndProjects`

## API Allocation needs (`/api/v1/allocation-needs`)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/allocation-needs?projectId={guid}` | Lista (filtro opcional por projeto) |
| GET | `/api/v1/allocation-needs/{id}` | Detalhe |
| POST | `/api/v1/allocation-needs` | Criar (exige `projectId` válido; skills do catálogo) |
| PUT | `/api/v1/allocation-needs/{id}` | Atualizar |
| DELETE | `/api/v1/allocation-needs/{id}` | Soft delete |

Status (RN-003): `Open`, `PartiallyFilled`, `Filled`  
Urgência: `Low`, `Medium`, `High`  
Criticidade: `Low`, `Medium`, `High`

Migration: `20260603170000_AddAllocationNeeds`

## API Allocations (`/api/v1/allocations`)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/allocations?personId=&projectId=` | Lista (filtros opcionais) |
| GET | `/api/v1/allocations/{id}` | Detalhe |
| POST | `/api/v1/allocations` | Criar |
| PUT | `/api/v1/allocations/{id}` | Atualizar |
| DELETE | `/api/v1/allocations/{id}` | Soft delete |

Status: `Planned`, `Confirmed`, `AtRisk`, `Closed`

**RN-001:** soma de `dedication_percent` por pessoa/semana (segunda–domingo) ≤ 100 — violação retorna **409**.  
**RN-002:** pessoa `Offboarded` ou `NoticePeriod` não recebe nova alocação — **409**.

Migration: `20260603180000_AddAllocations`

## Views e conflitos (Camada 1)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/projects/{id}/people` | Pessoas alocadas no projeto (agrupado por pessoa) |
| GET | `/api/v1/people/{id}/projects` | Projetos da pessoa (agrupado por projeto) |
| GET | `/api/v1/allocations/conflicts?personId=&projectId=&from=&to=` | Superalocações detectadas (RN-001), sem persistir tabela extra |

Sem migration adicional — leitura sobre `allocations` existentes.

## API Audit logs (`/api/v1/audit-logs`)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/audit-logs?entityType=&entityId=&action=&from=&to=` | Lista paginada de eventos |

Ações: `Created`, `Updated`, `Deleted` — geradas automaticamente no `SaveChanges` para entidades `TenantEntity`.

Header opcional (dev): `X-Actor-Id` — identificador gravado em `actor_id`.

Migration consolidada: `20260603190840_InitialCreate` (aplica todo o schema Camada 1)

**Camada 1 (backend) concluída.** Próximo: Fase 0 (auth mock, CI, frontend React).
