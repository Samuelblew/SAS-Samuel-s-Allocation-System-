# Sprint 09 — Frontend mobile e touch

Atualizado: 2026-06-20

## Objetivo

Tornar o IAS **utilizável em smartphones e tablets**: navegação acessível, layout fluido, formulários empilhados, tabelas roláveis e respeito a *safe areas* (notch / barra de gestos).

---

## Breakpoints adotados

| Faixa | Largura | Comportamento principal |
|-------|---------|-------------------------|
| Mobile | `< 640px` | Formulários em coluna, header compacto, KPIs menores |
| Tablet | `< 1024px` | Menu hamburger + drawer; subnav horizontal com scroll |
| Desktop | `≥ 1024px` | Nav horizontal no header; subnav com wrap |

Tailwind: `sm` = 640px, `lg` = 1024px — alinhado ao shell existente.

---

## Entregue nesta sprint

### Navegação mobile
- **`MobileNav`** — drawer lateral com backdrop, scroll interno, rodapé de preferências (idioma + tema)
- **`Layout`** — header dual: utilitários desktop (`ias-header-utilities`, `≥ 1024px`) vs controles compactos mobile (tema, dev, menu)
- **Correção crítica** — `.ias-header-nav { display: flex }` anulava `hidden lg:flex`; nav desktop aparecia no telefone. Visibilidade agora via `@media` em `index.css`, não Tailwind
- **i18n** — `layout.menu`, `layout.navMobile`, `layout.preferences`, `layout.language`; `common.close`

### Shell e layout (`index.css`)
- **Safe areas** — `env(safe-area-inset-*)` no `.ias-shell` e padding inferior do frame
- **Viewport** — `viewport-fit=cover` em `index.html`
- **Header compacto** — margens/padding reduzidos em `< 640px`
- **App frame** — `border-radius` e `100dvh` ajustados para telas pequenas

### Conteúdo rolável e touch
- **`DataTable`** — `.ias-table-wrap` com `overflow-x: auto`; tabela `min-width: 36rem`
- **Subnav** — scroll horizontal em tablet/mobile (`.ias-subnav`)
- **Formulários** — `.ias-form-row` empilha labels e botões em mobile
- **`PeriodFilter`** — classe `.ias-period-filter`; inputs em coluna no mobile
- **Listas/KPIs** — `.ias-page-list__item`, `.capacity-gaps-*` empilham em mobile
- **Dev settings** — painel fixo e largura total em telas estreitas
- **Touch** — `touch-action: manipulation` em botões de header e segmentos

### Componentes e páginas
- **`ThemeToggle`** — rótulo oculto em `< sm`; padding reduzido no header
- **`SimulationsPage`** — grid de resultados `grid-cols-1 sm:grid-cols-3`
- **`MatchingPage`** — campo de disponibilidade mínima `w-full sm:w-28`

---

## Checklist

- [x] Menu mobile (drawer) com i18n e a11y básica
- [x] Tabelas com scroll horizontal
- [x] Formulários e filtros de período responsivos
- [x] Subnav com scroll em tablet
- [x] Safe areas e viewport mobile
- [ ] **Acessibilidade avançada** — `aria` em tabelas, foco trap no drawer (futuro)
- [ ] **PWA** — manifest + service worker (futuro)
- [ ] **E2E mobile** — Playwright viewports (futuro)

---

## Convenções (novas telas)

```tsx
// Grids de KPI — preferir coluna única no mobile
<div className="grid grid-cols-1 gap-3 sm:grid-cols-3">…</div>

// Formulários — usar formRowClass (empilha sozinho no mobile)
<form className={formRowClass}>…</form>

// Listas largas — DataTable (scroll horizontal automático)
<DataTable>…</DataTable>

// Filtro de período — PeriodFilter (classe ias-period-filter)
<PeriodFilter from={…} to={…} … />
```

**Evitar:** larguras fixas (`w-28`, `min-w-[200px]`) sem fallback `w-full sm:…` em campos de formulário.

---

## Como testar

1. `cd Frontend && npm run dev`
2. DevTools → modo responsivo (iPhone / Pixel) ou dispositivo real na mesma rede
3. Verificar: abrir menu, navegar entre rotas, rolar tabelas, preencher formulário CRUD, alternar tema/idioma

---

## Referências

- [docs/MOBILE.txt](../MOBILE.txt) — guia resumido
- [docs/FRONTEND.txt](../FRONTEND.txt) — guia geral do frontend
- [07-FRONTEND-UX.md](./07-FRONTEND-UX.md) — design system
