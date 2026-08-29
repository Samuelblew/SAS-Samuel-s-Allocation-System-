# Diário de progresso

## 2026-06-21 — Cloud-ready: deploy demo Azure

### Entregue

- **API** — CORS via `Cors:AllowedOrigins`, Swagger via `FeatureFlags:EnableSwagger`, forwarded headers (App Service)
- **Config** — `appsettings.Production.json`, `infra/azure/app-settings.sample.json`
- **Frontend** — `staticwebapp.config.json`, `.env.production.example`
- **Scripts** — `scripts/deploy-api-azure.ps1`, `deploy-frontend-azure.ps1`, `apply-migrations-azure.ps1`
- **Doc** — [docs/DEPLOY-AZURE-DEMO.txt](../DEPLOY-AZURE-DEMO.txt)

---

## 2026-06-20 (b) — Frontend: correção header mobile

### Corrigido

- **Bug crítico** — `.ias-header-nav { display: flex }` em `index.css` anulava `hidden lg:flex` do Tailwind; nav desktop ("Painel", "Equipe e carteira"…) aparecia no telefone e estourava o header
- **Header mobile** — só logo + ícones (tema, dev, menu); idioma/tema no rodapé do drawer
- **Visibilidade nav** — controlada por media query CSS, não Tailwind

---

## 2026-06-20 — Frontend: compatibilidade mobile

### Entregue

- **MobileNav** — drawer com i18n (`layout.menu`, `layout.navMobile`), backdrop, Escape, scroll interno
- **Layout** — hamburger `< lg`, `aria-expanded` no botão menu
- **CSS** — safe areas, formulários empilhados, tabelas roláveis, subnav horizontal, touch targets
- **Componentes** — `PeriodFilter` (`.ias-period-filter`), `ThemeToggle` compacto, `DataTable` scroll
- **Páginas** — Simulations (grid KPI), Matching (input largura fluida)
- **index.html** — `viewport-fit=cover`
- **Documentação** — [09-FRONTEND-MOBILE.md](./09-FRONTEND-MOBILE.md), [docs/MOBILE.txt](../MOBILE.txt)

### Pendente

- Acessibilidade avançada, PWA, E2E mobile

---

## 2026-06-17 (b) — Frontend: Matching, Simulations, Financials, Allocations

### Entregue

- **Allocations** — RHF + Zod (`lib/schemas/allocation.ts`) + `EmptyState`
- **Matching** — i18n PT/EN via `pageMessages.ts` + componente `MatchingCandidateCard`
- **Simulations** — i18n completo + `SectionTitle`, `chipClass`, `formRowClass`
- **Financials** — i18n de alertas, tabelas, bench e simulação de margem
- **i18n** — `tp()` em `LocaleContext` para chaves de `pageMessages.ts`
- **Testes** — +3 (allocation schema) → 13 total

### Pendente

- Sidebar responsiva, acessibilidade, E2E

---

## 2026-06-17 — Frontend: qualidade, RHF/Zod, testes e CI

### Entregue

**Infraestrutura**
- React Hook Form + Zod + `@hookform/resolvers`
- Vitest + Testing Library (10 testes)
- CI `.github/workflows/frontend-ci.yml`
- `.gitignore` na raiz do monorepo

**Padrão de formulários**
- `lib/schemas/` (skill, person, project) + `FormField`
- Páginas migradas: **Skills**, **People**, **Projects**

**Design system**
- People, Projects, AllocationNeeds → `DataTable`, `LoadingState`, `EmptyState`
- Capacity, Financials → `StatCard` (removido `MiniStat` duplicado)
- `StatCard.highlight`: `'success' | 'warning' | false`

**i18n**
- +40 chaves PT/EN; interpolação `t(key, { var })`

**Documentação:** [08-FRONTEND-QUALIDADE.md](./08-FRONTEND-QUALIDADE.md), [docs/FRONTEND.txt](../FRONTEND.txt)

### Pendente

- i18n Matching, Simulations, Financials (seções internas)
- RHF em Allocations
- Sidebar responsiva

---

## 2026-06-03 — Tema claro: harmonia de cores

### Entregue

**Tokens e utilitários** (`index.css`)
- Chips semânticos (`ias-chip-success`, `ias-chip-warning`), segmentos de toggle (`ias-btn-segment--*`)
- Hover helpers (`hover:ias-text`, `hover:ias-text-danger`)
- Correção de `.ias-btn-link` quebrado na edição anterior

**Componentes**
- `StatusBadge` — 100% classes `ias-status-badge--*`
- `lib/ui.ts` — `btnSegmentActiveClass`, `btnSegmentInactiveClass`, `panelClass`

**Páginas**
- Substituição em massa de `slate-*`, `emerald-*`, `amber-*`, `indigo-*` hardcoded por tokens `ias-*`
- Matching, Simulations, Financials, Capacity, People, Projects, AllocationNeeds — botões primários/sucesso via `btnPrimaryClass` / `btnSuccessClass`
- Barras de progresso e badges de status alinhados ao design system

**Build:** `npm run build` OK

---

## 2026-06-16 — Frontend UX: auditoria e harmonização

### Entregue

**Auditoria** — mapeadas inconsistências entre shell (layout/tema/i18n) e 10 páginas de domínio ainda com classes Tailwind legadas.

**Design system**
- Componentes: `LoadingState`, `EmptyState`, `StatCard`, `SectionTitle`, `DataTable`, `TableActions`
- CSS: tabelas, stats, chips, botões link/success, `ias-form-row`
- `lib/ui.ts` estendido (`btnLinkClass`, `btnSuccessClass`, `chipClass`, …)

**Páginas harmonizadas (completas)**
- Skills, Conflicts, Dashboard, Allocations

**Harmonia em massa**
- `text-slate-*` → `ias-text-*` em todas as páginas
- Matching: alertas success via componente `Alert`
- i18n: chaves `common.add`, `empty.*`, `form.skills.*`, `alloc.prefillBanner`

**Documentação:** [07-FRONTEND-UX.md](./07-FRONTEND-UX.md)

### Pendente (ver checklist em 07-FRONTEND-UX.md)

- People, Projects, AllocationNeeds, Matching (cards), Capacity, Financials, Simulations
- i18n completo dos formulários
- Sidebar responsiva

---

## 2026-06-03 (j) — Batch matching por projeto (Camada 3 fechada)

### Entregue

- `GET /api/v1/projects/{id}/matching-candidates` — top candidatos por necessidade aberta
- `AllocationNeedCandidateRanker` — carga de capacidade compartilhada (single + batch)
- Frontend `/matching` — aba **Batch por projeto**
- **66 testes** passando — Camada 3 MVP concluída

### Próximo

- Cloud-ready: Dockerfile API + bicep
- Receita por pessoa alocada (opcional)

---

## 2026-06-03 (i) — Histórico de matching (Camada 3)

### Entregue

- Tabela `matching_suggestions` + migration `AddMatchingSuggestions`
- `POST /api/v1/allocation-needs/{id}/matching-suggestions` — registrar aceite/rejeição
- `GET /api/v1/allocation-needs/{id}/matching-suggestions` — listar histórico
- Frontend `/matching` — botões Aceitar/Rejeitar + seção histórico
- **65 testes** passando — Camada 3 ~98% (falta batch)

### Próximo

- Batch candidatos por projeto
- Cloud-ready: Dockerfile API + bicep

---

## 2026-06-03 (h) — Rentabilidade + filtros matching + edição necessidades

### Entregue

- `GET /api/v1/financials/profitability` — agregação por cliente ou tipo de projeto
- `FinancialProjectSummariesBuilder` — reutilizado por overview e profitability
- Matching: filtros `minAvailablePercent` e `excludePeopleOnProject`
- Frontend: rentabilidade em `/financials`, PUT em `/allocation-needs`, filtros em `/matching`
- **64 testes** passando — Camada 4 fechada no roadmap; Camada 3 ~92%

### Próximo

- Histórico de sugestões de matching (persistência)
- Receita por pessoa alocada
- Cloud-ready: Dockerfile API + bicep

---

## 2026-06-03 (g) — Camada 4 restante (bench + simulação margem)

### Entregue

- `GET /api/v1/financials/bench` — custo de capacidade ociosa
- `POST /api/v1/simulations/allocation-margin` — margem atual vs projetada
- Calculadores: `BenchCostCalculator`, `AllocationMarginSimulator`
- Frontend `/financials` — seção bench + formulário de simulação
- **61 testes** passando — Camada 4 MVP fechada (falta rentabilidade por cliente)

### Próximo

- Rentabilidade por cliente / tipo de projeto
- Filtros avançados matching + histórico de sugestões
- Edição de necessidades no frontend (PUT)
- Cloud-ready: Dockerfile API + bicep

---

## 2026-06-03 (f) — Frontend operacional

### Entregue

- `/allocation-needs` — CRUD criar/listar/excluir necessidades + skills obrigatórias
- `/simulations` — simulação de viabilidade de projeto (papéis dinâmicos)
- Menu e painel atualizados

### Próximo

- Camada 4: bench cost, simulação margem
- Edição de necessidades no frontend (PUT)

---

## 2026-06-03 (e) — Camada 2 future-gaps (tenant)

### Entregue

- `GET /api/v1/capacity/future-gaps?from=&to=`
- `FutureCapacityGapsCalculator` — demanda vs oferta semanal agregada
- Seção na página `/capacity` do frontend
- **57 testes** passando — Camada 2 MVP fechada

### Próximo

- Frontend: CRUD necessidades + simulação viabilidade
- Camada 4: bench cost, simulação margem

---

## 2026-06-03 (d) — Frontend Camadas 2–4

### Entregue

- `/capacity` — overview, skills, bench, subalocados
- `/matching` — candidatos por necessidade com breakdown RN-007
- `/financials` — overview, alertas margem, detalhe por projeto
- Painel atualizado com KPIs e links rápidos

### Próximo

- Cloud-ready: Dockerfile + bicep
- Camada 4 restante: bench cost, simulação margem

---

## 2026-06-03 (c) — Camada 2 restante (skill + RN-004)

### Entregue

- `GET /api/v1/capacity/skills-occupation` — ocupação agregada por skill
- RN-004: horas efetivas em `people/{id}/availability` e `capacity/overview`
- Calculadores: `EffectiveCapacityCalculator`, `SkillOccupationCalculator`
- **55 testes** passando

### Próximo

- Frontend: capacity, matching, financeiro
- Gaps futuros de capacidade (visão tenant)
- Cloud-ready: Dockerfile + bicep

---

## 2026-06-03 (b) — Camada 4 financeiro (RN-005 / RN-006)

### Entregue

- `GET /api/v1/projects/{id}/financials` — custo por alocação + margem
- `GET /api/v1/financials/overview` — visão agregada + alertas de margem baixa
- Calculadores: `AllocationCostCalculator`, `ProjectMarginCalculator`
- **49 testes** passando

### Documentação

- `docs/roadmap/06-CAMADA-4.md` criado
- `03-CAMADAS-2-4.md` atualizado

### Próximo

- Camada 2 restante: ocupação por skill, RN-004
- Camada 4 restante: bench cost, simulação de margem
- Frontend: telas financeiras e matching

---

## 2026-06-03 — Camada 3 matching (RN-007)

### Entregue

- `GET /api/v1/allocation-needs/{id}/candidates?maxResults=20`
- `AllocationNeedCandidateMatcher` — scoring determinístico com breakdown
- Query/handler `GetAllocationNeedCandidates`
- Testes: `AllocationNeedCandidateMatcherTests`, `MatchingApiTests`
- **43 testes** passando

### Documentação

- `docs/roadmap/05-CAMADA-3.md` criado
- `03-CAMADAS-2-4.md` atualizado

### Próximo

- Camada 4: financeiro (`/projects/{id}/financials`, RN-005/RN-006)
- Camada 2 restante: ocupação por skill, RN-004
- Frontend: tela de matching por necessidade

---

## 2026-06-04 (b) — Camada 2 capacidade

### Entregue

- `GET /api/v1/capacity/overview` — semanas + times
- `GET /api/v1/capacity/bench` — bench futuro
- `GET /api/v1/capacity/projects-understaffed`
- `GET /api/v1/projects/understaffed`
- `POST /api/v1/simulations/project-feasibility`
- Calculadores: `CapacityOverviewCalculator`, `ProjectFeasibilitySimulator`
- **40 testes** passando

### Documentação

- `docs/roadmap/04-CAMADA-2.md` criado
- `03-CAMADAS-2-4.md` atualizado

### Próximo

- Camada 2 restante: ocupação por skill, RN-004
- Camada 3: matching `/allocation-needs/{id}/candidates`

---

## 2026-06-04 — Fase 0 + Camada 1 (backend)

### Entregue

**Fase 0:** Tenant, User, JWT mock, health+DB, CI  
**Camada 1:** RN-003, disponibilidade, staffing gaps  
**Testes:** 34 → 40 após Camada 2
