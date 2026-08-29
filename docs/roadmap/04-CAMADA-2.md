# Sprint 04 — Camada 2 (capacidade)

Atualizado: 2026-06-04

## Objetivo

Planejamento de capacidade: visão agregada, bench, projetos subalocados e simulação comercial.

## Entregue

| # | Endpoint | Descrição |
|---|----------|-----------|
| 1 | `GET /capacity/overview?from=&to=` | Ocupação semanal agregada + ocupação por time |
| 2 | `GET /capacity/bench?from=&to=` | Pessoas em bench (disponibilidade ≥ 50% padrão) |
| 3 | `GET /capacity/projects-understaffed` | Projetos com necessidades não preenchidas |
| 4 | `GET /projects/understaffed` | Alias do item 3 |
| 5 | `POST /simulations/project-feasibility` | Simula viabilidade de staffing por papéis |

## Já existia (sprint anterior)

- `GET /capacity/people-available`
- `GET /capacity/people/{id}/availability`
- `GET /capacity/projects/{id}/staffing-gaps`
- Conflitos via `/allocations/conflicts`

## Entregue (sprint 04b — 2026-06-03)

| # | Endpoint / mudança | Descrição |
|---|-------------------|-----------|
| 6 | `GET /capacity/skills-occupation?from=&to=` | Ocupação agregada por skill (% + horas) |
| 7 | RN-004 em `/capacity/people/{id}/availability` | `allocatedHours`, `availableHours` por semana |
| 8 | RN-004 em `/capacity/overview` | `totalCapacityHours`, `totalAllocatedHours`, `totalAvailableHours` |

## Entregue (sprint 04c — 2026-06-03)

| # | Endpoint | Descrição |
|---|----------|-----------|
| 9 | `GET /capacity/future-gaps?from=&to=` | Gaps futuros agregados do tenant (demanda vs oferta semanal) |

## Camada 2 — concluída no MVP

## Simulação — regra MVP

Por papel simulado: conta candidatos ativos com skills/senioridade compatíveis e `minAvailablePercent >= dedication` no período. Se candidatos ≥ quantidade → papel atendido. Escaneia até 26 semanas para `earliestFeasibleStart`.
