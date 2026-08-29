# Sprint 07 — Frontend UX e harmonia visual

Atualizado: 2026-06-20

## Objetivo

Tornar o frontend **profissional e coerente** para gestores de projeto (GP): design system único, layout claro, tema claro/escuro, i18n PT/EN, componentes reutilizáveis.

---

## Diagnóstico (auditoria 2026-06-16)

### O que já estava harmonizado
- Shell: sidebar agrupada, topbar com breadcrumb, tema e idioma
- Tokens CSS `ias-*` em `index.css`
- Títulos de página via i18n + `PageHeader hideTitle`
- Dashboard, Layout, Alert, Card, PeriodFilter

### Problemas encontrados
| Problema | Impacto |
|----------|---------|
| Páginas CRUD com `slate-*` / `indigo-*` hardcoded | Visual quebrado no tema claro |
| `btnPrimaryClass` / `labelClass` definidos mas não usados | Botões e labels inconsistentes |
| Tabelas HTML duplicadas com estilos diferentes | Listas parecem “outro app” |
| Loading / empty states ad hoc | UX irregular |
| Corpo das telas 100% PT fixo | Switch EN só traduz menu |
| `MiniStat` duplicado (Capacity, Financials) | Manutenção difícil |
| Matching com banners `emerald-*` custom | Fora do design system |

---

## Entregue nesta sprint

### Design system (`Frontend/src/`)
- `index.css` — tabelas (`.ias-table-*`), stats, chips, botões link/success, form-row
- `lib/ui.ts` — `btnSuccessClass`, `btnLinkClass`, `formRowClass`, `chipClass`

### Componentes compartilhados
| Componente | Função |
|------------|--------|
| `LoadingState` | Carregando padronizado + i18n |
| `EmptyState` | Lista vazia em Card |
| `StatCard` | KPI do painel (extraído do Dashboard) |
| `SectionTitle` | Título de seção em formulários |
| `DataTable` | Tabela harmonizada |
| `TableActions` | Editar / Excluir com i18n |

### Páginas migradas (referência completa)
- **Skills** — formulário, tabela, empty, i18n
- **Conflicts** — cards, empty success, tokens
- **Dashboard** — `StatCard` compartilhado
- **Allocations** — banner info, form, tabela, botões

### Harmonia em massa
- Substituição `text-slate-*` → `ias-text-*` em todas as páginas
- Links `indigo-*` → `ias-link`
- Matching: feedback success via `Alert variant="success"`

### i18n (`messages.ts`)
- Novas chaves: `common.add`, `common.actions`, `form.skills.*`, `empty.*`, `alloc.prefillBanner`

---

## Checklist pendente

- [x] **People** — migrar para `DataTable` + RHF/Zod + i18n
- [x] **Projects** — unificar botões cliente/projeto; `DataTable` + RHF/Zod
- [x] **AllocationNeeds** — `chipClass`, `formRowClass`, `DataTable`, i18n
- [x] **Capacity / Financials** — `StatCard` compartilhado; remover `MiniStat` local
- [x] **Matching** — i18n + `MatchingCandidateCard`
- [x] **Simulations** — i18n + design system
- [x] **Financials** — i18n seções internas
- [x] **Allocations** — RHF + Zod
- [ ] **StatusBadge** — revisar mapeamento de status em necessidades
- [x] **Responsivo** — menu mobile (drawer), tabelas roláveis, formulários empilhados
- [ ] **Acessibilidade** — `aria` em tabelas e switches

> Sprint 08 (qualidade): ver [08-FRONTEND-QUALIDADE.md](./08-FRONTEND-QUALIDADE.md)

---

## Convenções (para novas telas)

```tsx
import { btnGhostClass, btnPrimaryClass, formRowClass, inputClass, labelClass } from '../lib/ui'
import { LoadingState } from '../components/LoadingState'
import { SectionTitle } from '../components/SectionTitle'
import { useLocale } from '../context/LocaleContext'

// Formulário horizontal
<form className={formRowClass}>
  <label className={labelClass}>{t('...')}<input className={inputClass} /></label>
  <button className={btnPrimaryClass}>{t('common.save')}</button>
</form>

// Lista
{loading && <LoadingState />}
{empty && <EmptyState message={t('empty.x')} />}
<DataTable>...</DataTable>
```

**Não usar:** `bg-slate-*`, `text-white`, `bg-indigo-600` inline nas páginas.

---

## Próximo passo sugerido

1. Migrar **People** e **Projects** (mais usados pelo GP)
2. Completar i18n dos formulários
3. Polish **Matching** (maior complexidade visual)
