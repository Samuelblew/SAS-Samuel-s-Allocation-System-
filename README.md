# IAS — Intelligent Allocation System

Plataforma de alocação de pessoas em consultorias: visibilidade operacional, capacidade futura e apoio à decisão comercial.

**Cloud alvo:** Microsoft Azure (futuro).

## Documentação

| Arquivo | Conteúdo |
|---------|----------|
| [docs/REQUISITOS_PRODUTO.txt](docs/REQUISITOS_PRODUTO.txt) | O que o produto deve fazer (texto original — referência) |
| [docs/ARQUITETURA.txt](docs/ARQUITETURA.txt) | Stack, domínio, APIs, roadmap técnico |
| [docs/DESENVOLVIMENTO.txt](docs/DESENVOLVIMENTO.txt) | Guia para dev/IA: estado, checklist, convenções |
| [docs/FRONTEND.txt](docs/FRONTEND.txt) | Guia do frontend: stack, padrões, testes, CI |
| [docs/DEPLOY-AZURE-DEMO.txt](docs/DEPLOY-AZURE-DEMO.txt) | Deploy demo: SWA + App Service + MySQL |

**Para agentes de IA:** ler `docs/DESENVOLVIMENTO.txt` antes de codar.

## Status atual

- **Fase:** 0 (Fundação) — backend iniciado em [`Backend/`](Backend/)
- **Ambiente:** pronto ( .NET 10, Node 22, MySQL 8.0.46, banco `ias_dev` )
- **Camada 1 (backend):** concluída (CRUD, alocações, views, conflitos, audit log)
- **Frontend:** [`Frontend/`](Frontend/) — React 19 + Vite 6 (dev em http://localhost:5173)
- **Backend Fase 0:** concluída (tenant, user, JWT, CI, RN-003, capacity básico)
- **Roadmap:** [`docs/roadmap/`](docs/roadmap/)
- **Próximo passo:** Camada 2 (capacidade) + evoluir Frontend

## Estado atual (MVP) — decisões conscientes

Este projeto está em fase de MVP. As simplificações abaixo são intencionais, não lacunas percebidas tarde — cada uma tem um motivo para a fase atual e um caminho já mapeado para evoluir.

| Decisão atual | Por que faz sentido no MVP | Evolução planejada |
|----------------|------------------------------|----------------------|
| Autenticação mock (`POST /auth/dev-token`, sem hash de senha, sem `[Authorize]` nas rotas) | Validar o domínio (alocação, capacidade, matching) antes de fechar autenticação completa | Login real com hash de senha e `[Authorize]` por rota (Fase 0 em andamento) |
| Tenant resolvido pelo header `X-Tenant-Id` | Permite testar multi-tenant sem depender de login pronto | Tenant extraído de claim no JWT — isolamento não depende do cliente informar o header corretamente |
| Testes de integração com EF Core InMemory | Ciclo de feedback rápido no CI | Testcontainers + MySQL real, para validar FK, unique index e tradução de SQL que o InMemory não checa |
| Detecção de sobrealocação (RN-001) calculada em memória (LINQ sobre as alocações carregadas) | Suficiente na escala de uma demo | Mover para agregação no banco quando o volume de dados justificar |
| Sem dado de exemplo (seed) e sem deploy público ainda | Foco até aqui foi Camada 1 (backend) e Fase 0 | Seed de demonstração + deploy no Azure (scripts já existem em [`scripts/`](scripts/) e [`docs/DEPLOY-AZURE-DEMO.txt`](docs/DEPLOY-AZURE-DEMO.txt)) |

## Stack resumida

- Backend: [`Backend/`](Backend/) — .NET 10, EF Core 10, MySQL (Microting provider em dev), Clean Architecture
- Frontend: [`Frontend/`](Frontend/) — React 19, Vite 6, TypeScript, Tailwind, TanStack Query
- Banco local: MySQL 8 (`ias_dev` em localhost:3306)
- Sem Docker no desenvolvimento local
