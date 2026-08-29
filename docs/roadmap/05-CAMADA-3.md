# Sprint 05 — Camada 3 (matching RN-007)

Atualizado: 2026-06-03

## Objetivo

Ranking determinístico de candidatos para uma necessidade de alocação, com score explicado (breakdown).

## Entregue

| # | Endpoint | Descrição |
|---|----------|-----------|
| 1 | `GET /allocation-needs/{id}/candidates?maxResults=20` | Lista candidatos ordenados por score total |
| 2 | Filtros `minAvailablePercent`, `excludePeopleOnProject` | Refina candidatos antes do ranking |
| 3 | `POST/GET /allocation-needs/{id}/matching-suggestions` | Histórico de aceite/rejeição |
| 4 | `GET /projects/{id}/matching-candidates` | Batch — candidatos para todas as necessidades abertas |

## Scoring (RN-007)

| Fator | Peso máx. | Notas |
|-------|-----------|-------|
| Disponibilidade | 25 | `minAvailablePercent / dedication` no período |
| Skills obrigatórias | 25 | % de skills requeridas presentes |
| Skills desejáveis | 10 | % de skills desejadas presentes |
| Senioridade | 15 | Match exato ou parcial com `expectedSeniority` |
| Histórico | 10 | Mesmo papel e/ou mesmo `projectType` em alocações ativas |
| Custo | 10 | Normalizado vs baseline do tenant (menor custo = melhor) |
| Penalidade superalocação | −20 | RN-001: excede 100% ou capacidade semanal |
| Penalidade troca de projeto | −10 | Muitos projetos distintos nos últimos 12 meses |

## Código

- `IAS.Application/Matching/AllocationNeedCandidateMatcher.cs`
- `IAS.Application/Matching/Queries/GetAllocationNeedCandidates/`
- DTOs API: `IAS.Api/Dtos/MatchingDtos.cs`

## Testes

- `AllocationNeedCandidateMatcherTests` (unitário)
- `MatchingApiTests` (integração)

## Pendente Camada 3 (fora do MVP técnico)

- Sugestão de alocação alternativa (requisito de produto, não especificado no roadmap técnico)
