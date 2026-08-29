# Sprint 02 — Camada 1 (lacunas backend)

## Objetivo

Completar no backend o que o requisito de produto pede na Camada 1 além do CRUD já existente.

## Itens

| # | Item | Endpoint |
|---|------|----------|
| 1 | Disponibilidade por pessoa | `GET /people/{id}/availability?from=&to=` |
| 2 | Gaps de staffing | `GET /projects/{id}/staffing-gaps` |
| 3 | RN-003 | Automático nos handlers de alocação |

## Disponibilidade (regra)

Por semana no período:
- `allocatedPercent` = soma de dedicação de alocações ativas (não `Closed`)
- Se indisponibilidade cobre a semana → `availablePercent = 0`
- Senão → `availablePercent = max(0, 100 - allocatedPercent)`

## Staffing gaps

Por `AllocationNeed` do projeto:
- `coveredPercent` = soma de alocações mesmo `role` com overlap de datas
- `gapPercent` = `need.DedicationPercent - coveredPercent`
- `status` derivado (RN-003)
