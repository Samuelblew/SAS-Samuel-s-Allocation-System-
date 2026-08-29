# Roadmap técnico — IAS

Atualizado: 2026-06-20

## Estado de partida

- Camada 1–4 backend: concluídas (66 testes)
- Frontend: React 19 + design system + RHF/Zod + CI (sprint 08)
- Camada 5 (IA): fora de escopo

## Ordem de execução

```
Fase 0 (fundação)
  ├── Tenant + User (entidades, API bootstrap)
  ├── Auth mock JWT (dev) + fallback X-Tenant-Id
  ├── Health com checagem de banco
  ├── RN-003 (status AllocationNeed automático)
  └── CI (GitHub Actions: build + test)

Camada 1 — completar backend
  ├── Disponibilidade por pessoa/período
  ├── Gaps de staffing por projeto
  └── Cobertura de necessidades vs alocações

Camada 2 — capacidade
  ├── Ocupação semanal/mensal
  ├── Pessoas disponíveis por período
  ├── Bench / ociosidade
  ├── Projetos sem equipe completa (agregado)
  └── Simulação de viabilidade de projeto

Camada 3 — matching
  └── Ranking determinístico + breakdown de score

Camada 4 — financeiro
  └── Custo, margem, alertas por projeto/cliente

Cloud-ready (demo Azure)
  ├── CORS + Swagger configuráveis (App Settings) — concluído
  ├── Scripts deploy (App Service + SWA + migrations) — concluído
  ├── docs/DEPLOY-AZURE-DEMO.txt — concluído
  └── Entra ID (substitui mock JWT) — futuro

Frontend (sprints 07–09)
  ├── Design system + i18n PT/EN (07 — concluído)
  ├── RHF + Zod + testes + CI (08 — concluído)
  └── Mobile + touch (09 — concluído)
```

## Definition of Done (slice backend)

- Domain + Handler + Validator + Repository + Migration + Controller Swagger
- Teste domínio ou application quando houver regra
- Teste integração API + isolamento tenant
- Entrada em PROGRESS.md
