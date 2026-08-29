# IAS Web

Interface React 19 + Vite 6 para a API IAS (Camadas 1–4).

## Pré-requisitos

- Node.js 20+
- API rodando em `http://localhost:5203` (perfil `http` do `IAS.Api`)

## Desenvolvimento

```bash
cd Frontend
npm install
npm run dev
```

Abra **http://localhost:5173**. O Vite faz proxy de `/api` para a API local.

## Scripts

| Comando | Descrição |
|---------|-----------|
| `npm run dev` | Servidor de desenvolvimento |
| `npm run build` | Typecheck + build de produção |
| `npm run test` | Testes (Vitest) |
| `npm run test:watch` | Testes em modo watch |
| `npm run lint` | ESLint |
| `npm run preview` | Preview do build |

## Configuração

| Variável | Uso |
|----------|-----|
| `VITE_API_URL` | URL base da API (opcional). Vazio = proxy do Vite. Ex.: `http://localhost:5203` |

Copie `.env.example` para `.env` se precisar apontar direto para a API (sem proxy).

## Tenant

Toda requisição exige o header `X-Tenant-Id`. Informe um GUID válido no campo **Tenant** no topo da UI (persistido em `localStorage`).

Opcional: `X-Actor-Id` para auditoria.

## Padrões de código

- **Formulários:** React Hook Form + Zod (`lib/schemas/`)
- **Listas:** `DataTable`, `LoadingState`, `EmptyState`
- **Estilo:** tokens `ias-*` via `lib/ui.ts` — ver `docs/FRONTEND.txt`
- **Textos:** i18n via `useLocale().t('chave')`

Documentação completa: [`docs/FRONTEND.txt`](../docs/FRONTEND.txt)

## Páginas

- **Painel** — health, contagens e KPIs (conflitos, subalocados, alertas margem)
- **Skills**, **Pessoas**, **Projetos**, **Necessidades**, **Alocações**, **Simulação**, **Conflitos**
- **Capacidade** — overview semanal, times, skills, bench, subalocados
- **Matching** — ranking de candidatos por necessidade (RN-007)
- **Financeiro** — overview, alertas de margem, detalhe por projeto (RN-005/006)

## CI

O workflow `.github/workflows/frontend-ci.yml` executa lint, testes e build em cada alteração em `Frontend/`.

## Build

```bash
npm run build
npm run preview
```
