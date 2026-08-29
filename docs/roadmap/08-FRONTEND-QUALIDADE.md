# Sprint 08 — Qualidade e padrões do frontend

Atualizado: 2026-06-17

## Objetivo

Alinhar o frontend às convenções documentadas em `docs/DESENVOLVIMENTO.txt`: **React Hook Form + Zod**, testes automatizados, CI, design system consistente e i18n nas telas principais.

---

## Diagnóstico (auditoria 2026-06-17)

| Problema | Impacto |
|----------|---------|
| Convenção RHF + Zod documentada mas não implementada | Formulários frágeis, validação só no submit manual |
| Sem testes frontend | Regressões silenciosas |
| Sem CI do frontend | Build quebrado só descoberto localmente |
| People / Projects com tabela HTML custom | Visual fora do design system |
| MiniStat duplicado (Capacity, Financials) | Manutenção duplicada |
| Textos PT hardcoded em formulários | Switch EN incompleto |
| Sem `.gitignore` na raiz do monorepo | Risco de lixo versionado |

---

## Entregue nesta sprint

### Infraestrutura
- **Dependências:** `react-hook-form`, `zod`, `@hookform/resolvers`
- **Testes:** Vitest + Testing Library + jsdom
- **CI:** `.github/workflows/frontend-ci.yml` (lint + test + build)
- **Raiz:** `.gitignore` unificado

### Padrão de formulários
- `lib/schemas/` — skill, person, project (+ `to*Payload`)
- `components/forms/FormField.tsx` — label + erro
- `lib/errors.ts` — `getErrorMessage()`
- **Páginas migradas para RHF + Zod:** Skills, People, Projects

### Design system e UX
- **People, Projects, AllocationNeeds** → `DataTable`, `LoadingState`, `EmptyState`
- **Capacity, Financials** → `StatCard` compartilhado (removido `MiniStat` local)
- **StatCard** — variantes `highlight: 'success' | 'warning' | false`
- **AllocationNeeds** — `formRowClass`, `chipClass`, `SectionTitle`

### i18n
- +40 chaves em `messages.ts` (formulários, colunas, KPIs capacity/financials)
- `t(key, { var })` — interpolação em `LocaleContext`

### Testes (13 casos)
- `lib/errors.test.ts`
- `lib/schemas/skill.test.ts`
- `lib/schemas/person.test.ts`
- `components/LoadingState.test.tsx`

### Documentação
- `docs/FRONTEND.txt` — guia completo do frontend
- `Frontend/README.md` — atualizado
- `docs/DESENVOLVIMENTO.txt` — referência ao guia frontend

---

## Checklist pendente

- [x] **Matching** — i18n completo + `MatchingCandidateCard`
- [x] **Simulations** — i18n + harmonização design system
- [x] **Financials** — i18n das seções internas
- [x] **Allocations** — RHF + Zod
- [x] **Mobile** — drawer, tabelas roláveis, safe areas (ver [09-FRONTEND-MOBILE.md](./09-FRONTEND-MOBILE.md))
- [ ] **Acessibilidade** — `aria` em tabelas e switches
- [ ] **E2E** — Playwright (futuro)

---

## Convenções (atualizadas)

```tsx
// Schema
import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { personFormSchema, toPersonPayload } from '../lib/schemas/person'
import { FormField } from '../components/forms/FormField'
import { getErrorMessage } from '../lib/errors'

const form = useForm({ resolver: zodResolver(personFormSchema), defaultValues })

// Mutation
onError: (err) => setFormError(getErrorMessage(err))

// Lista
{loading && <LoadingState />}
{empty && <EmptyState message={t('empty.people')} />}
<DataTable>...</DataTable>
```

---

## Próximo passo sugerido

1. Completar i18n em Matching e Simulations
2. Migrar Allocations para RHF
3. Cloud-ready: Static Web Apps + build de produção no CI
