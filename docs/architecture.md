# Architecture

## Folder Structure

```
jeddah-superdom/
├── .husky/                 # Git hooks (pre-commit, pre-push)
├── docs/                   # Documentation
├── public/                 # Static assets
├── src/
│   ├── @core/              # Template base layer (components, utils, SCSS)
│   │   ├── components/     # Shared UI components (Search, Pagination, Charts)
│   │   ├── composables/    # Theme, skins composables
│   │   ├── scss/           # Base styles, variables, mixins
│   │   └── utils/          # Validators, formatters, helpers
│   │
│   ├── @layouts/           # Layout system
│   │   ├── components/     # VerticalNav, VerticalNavLink, etc.
│   │   ├── composable/     # useLayouts
│   │   └── styles/         # Layout SCSS
│   │
│   ├── components/         # Global app components
│   │   ├── AppHeader.vue
│   │   ├── AppBreadcrumb.vue
│   │   ├── CompanyCard.vue
│   │   ├── dataTable.vue
│   │   └── icons/          # SVG icon components
│   │
│   ├── composables/        # Shared composable functions
│   │   ├── useTablePage.ts     # Pagination + search + loading
│   │   ├── useConfirmDialog.ts # Confirmation dialogs
│   │   ├── useFileUpload.ts    # File upload handling
│   │   └── useBreadcrumbs.ts   # Breadcrumb generation
│   │
│   ├── config/             # Environment configuration
│   │   ├── env.ts          # Typed env vars with validation
│   │   └── index.ts        # App config
│   │
│   ├── layouts/            # Page layouts
│   │   ├── DashboardLayout.vue  # Main admin layout (sidebar + topbar)
│   │   └── blank.vue           # Blank layout (auth pages)
│   │
│   ├── locales/            # Translation files
│   │   ├── ar.json         # Arabic (default)
│   │   └── en.json         # English
│   │
│   ├── navigation/         # Sidebar navigation items
│   │   └── vertical/index.ts
│   │
│   ├── pages/              # Route pages
│   │   ├── auth/           # Login, OTP, password reset
│   │   ├── company/        # Company management
│   │   ├── home/           # Dashboard home
│   │   ├── notifications/  # Notification management
│   │   ├── pilgrims/       # Pilgrim management
│   │   ├── roles/          # Role & permission management
│   │   └── users/          # User management
│   │
│   ├── plugins/            # Vue plugins
│   │   ├── axios.ts        # Axios instance with interceptors
│   │   ├── casl/           # CASL ability definitions
│   │   ├── i18n.ts         # vue-i18n setup
│   │   └── vuetify/        # Vuetify theme, defaults, icons
│   │
│   ├── router/             # Vue Router
│   │   ├── index.ts        # Router instance
│   │   └── routes.ts       # Route definitions
│   │
│   ├── services/           # API service layer
│   │   ├── base.service.ts     # Abstract HTTP wrapper
│   │   ├── auth.service.ts     # Auth (login, logout, OTP)
│   │   ├── companies.service.ts
│   │   ├── dashboard.service.ts
│   │   ├── flights.service.ts
│   │   ├── notifications.service.ts
│   │   ├── pilgrims.service.ts
│   │   ├── roles.service.ts
│   │   └── users.service.ts
│   │
│   ├── stores/             # Pinia stores
│   │   ├── auth.store.ts       # Auth state + CASL
│   │   ├── app.store.ts        # App state (snackbar, theme)
│   │   └── settings.store.ts   # Settings
│   │
│   ├── styles/             # Global SCSS
│   │   ├── styles.scss
│   │   └── variables/
│   │
│   └── types/              # TypeScript interfaces
│       └── index.ts        # All API types (must match .NET DTOs)
│
├── .env.example            # Environment variable template
├── .eslintrc.js            # ESLint config
├── .prettierrc             # Prettier config
├── tsconfig.json           # TypeScript config
└── vite.config.mjs         # Vite + Vitest config
```

## Data Flow

```
Component → Composable → Service → Axios → Backend API
                ↓
            Pinia Store (optional state)
```

1. **Component** calls composable method (e.g., `fetchData()`)
2. **Composable** (e.g., `useTablePage`) manages state and calls service
3. **Service** (e.g., `companiesService.getAll()`) makes HTTP request via `BaseService`
4. **BaseService** wraps axios with interceptors (auth, error handling)
5. Response flows back through the chain

## Key Design Decisions

- **No raw axios in components** — Always use service layer
- **Composition API only** — No Options API anywhere
- **Arabic-first** — Default locale is Arabic, RTL layout
- **TypeScript strict** — No `any`, no `!` assertions (except env declarations)
- **Template base layer** — `@core/` and `@layouts/` are from the Vuetify template (exempt from strict linting)
