# Sprint 06 — Camada 4 (financeiro RN-005 / RN-006)

Atualizado: 2026-06-03

## Objetivo

Visibilidade de custo de alocação e margem por projeto, com alertas de margem baixa.

## Entregue

| # | Endpoint | Descrição |
|---|----------|-----------|
| 1 | `GET /projects/{id}/financials?from=&to=&marginAlertThreshold=15` | Custo por alocação + margem do projeto |
| 2 | `GET /financials/overview?from=&to=&marginAlertThreshold=15` | Visão agregada + alertas RN-006 |
| 3 | `GET /financials/bench?from=&to=&minAvailablePercent=50` | Custo de capacidade ociosa (bench) |
| 4 | `POST /simulations/allocation-margin` | Simular margem do projeto antes de alocar |
| 5 | `GET /financials/profitability?groupBy=client\|projectType` | Rentabilidade agregada por cliente ou tipo |

## Regras

### RN-005 — Custo de alocação

Por alocação ativa (status ≠ `closed`), no período:

```
horas_semana = weekly_capacity_hours × (dedication_percent / 100)
custo_semana = horas_semana × hourly_rate
custo_total  = soma das semanas no período
```

`hourly_rate` = `hourly_cost` da pessoa, ou `monthly_cost / 160` quando só custo mensal.

### RN-006 — Margem de projeto

```
receita = estimated_revenue ?? budget
margem  = receita - custo_total_alocações
margem% = margem / receita × 100
alerta  = margem% < marginAlertThreshold (padrão 15%)
```

## Código

- `IAS.Application/Financial/AllocationCostCalculator.cs`
- `IAS.Application/Financial/ProjectMarginCalculator.cs`
- `IAS.Application/Financial/BenchCostCalculator.cs`
- `IAS.Application/Financial/AllocationMarginSimulator.cs`
- `IAS.Application/Financial/Queries/GetProjectFinancials/`
- `IAS.Application/Financial/Queries/GetFinancialOverview/`
- `IAS.Application/Financial/Queries/GetBenchCost/`
- `IAS.Application/Financial/Commands/SimulateAllocationMargin/`
- `IAS.Infrastructure/Financial/FinancialReadRepository.cs`

## Testes

- `AllocationCostCalculatorTests`, `ProjectMarginCalculatorTests` (unitário)
- `BenchCostCalculatorTests`, `AllocationMarginSimulatorTests` (unitário)
- `FinancialApiTests` — overview, bench, simulação margem (integração)

## Frontend

- `/financials` — bench, simulação margem, rentabilidade agregada

## Pendente Camada 4

- Receita por pessoa alocada (granularidade opcional)
