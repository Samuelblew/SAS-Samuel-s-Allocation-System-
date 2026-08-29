# Sprints futuros — Camadas 2, 3 e 4

## Camada 2 — Capacidade

- [x] `GET /capacity/overview?from=&to=`
- [x] `GET /capacity/people-available?from=&to=&minAvailablePercent=`
- [x] `GET /capacity/bench?from=&to=`
- [x] `GET /projects/understaffed` (+ `/capacity/projects-understaffed`)
- [x] `POST /simulations/project-feasibility`
- [x] Ocupação por skill (agregado) — `GET /capacity/skills-occupation`
- [x] RN-004 capacidade efetiva em horas (availability + overview)
- [x] Gaps futuros de capacidade (visão tenant) — `GET /capacity/future-gaps`

## Camada 3 — Matching

- [x] `GET /allocation-needs/{id}/candidates` com score e breakdown (RN-007)
- [x] Filtros opcionais (`minAvailablePercent`, `excludePeopleOnProject`)
- [x] Histórico de sugestões aceitas/rejeitadas (`POST/GET .../matching-suggestions`)
- [x] Batch: `GET /projects/{id}/matching-candidates`

## Camada 4 — Financeiro

- [x] `GET /projects/{id}/financials` (RN-005)
- [x] `GET /financials/overview` + alertas margem baixa (RN-006)
- [x] `GET /financials/bench` — custo de bench
- [x] `POST /simulations/allocation-margin` — simulação de margem
- [x] `GET /financials/profitability` — rentabilidade por cliente / tipo de projeto

## Dependências

Camada 3 depende de Camada 2 (disponibilidade).  
Camada 4 depende de alocações + custos já cadastrados.
