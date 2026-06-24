# Jeddah Superdome — Admin Dashboard

Vue 3 SPA for managing pilgrims, companies, flights, and notifications.

## Tech Stack

| Layer | Technology |
|-------|------------|
| Framework | Vue 3 (Composition API, `<script setup>`) |
| UI | Vuetify 3 |
| Language | TypeScript (strict) |
| State | Pinia |
| Routing | Vue Router (file-based) |
| HTTP | Axios (service layer) |
| Auth | CASL + JWT |
| Real-time | SignalR |
| i18n | vue-i18n (Arabic, RTL) |
| Testing | Vitest, Playwright |
| Build | Vite 5 |
| Linting | ESLint + Prettier + Husky |

## Project Structure

```
src/
├── @core/            Shared components, utils, SCSS
├── @layouts/         Layout system
├── components/       Global components
├── composables/      Shared composables (useTablePage, useConfirmDialog, useFileUpload)
├── config/           Environment config
├── layouts/          Page layouts
├── locales/          i18n translations
├── navigation/       Sidebar nav items
├── pages/            Route pages
├── plugins/          Axios, CASL, i18n, Vuetify setup
├── router/           Route definitions
├── services/         API calls (extends BaseService)
├── stores/           Pinia stores (Composition API)
├── styles/           Global SCSS
└── types/            TypeScript interfaces
```

## Key Patterns

- **Services** — All API calls in `src/services/`. Never raw axios in components.
- **Composables** — `useTablePage<T>` for paginated tables, `useConfirmDialog` for confirmations, `useFileUpload` for uploads.
- **Stores** — Thin Pinia stores, delegate to services.
- **Types** — All interfaces in `src/types/index.ts`, matching .NET DTOs.

## Commands

| Command | Description |
|---------|-------------|
| `npm run dev` | Dev server (localhost:5173) |
| `npm run build` | Production build |
| `npm run preview` | Preview production build |
| `npm run lint` | ESLint fix |
| `npm run type-check` | vue-tsc check |
| `npm run test` | Vitest unit tests |
| `npm run test:coverage` | Tests with coverage |
| `npm run test:e2e` | Playwright E2E tests |

## Environment

```env
VITE_API_BASE_URL=http://localhost:5200/api/v1/dashboard
```

The dev server proxies `/api` and `/hubs` to the backend via `vite.config.mjs`.

## Portals

1. **Admin** — Full access to all features
2. **Company** — Limited to own pilgrims and flights
