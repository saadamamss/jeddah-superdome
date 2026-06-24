# Deployment

## Vercel (Recommended)

### Setup

1. Push to GitHub
2. Import project on [vercel.com/new](https://vercel.com/new)
3. Vercel auto-detects Vite — no config needed
4. Add environment variables in Vercel dashboard

### Environment Variables

| Variable                   | Description          | Example                                    |
| -------------------------- | -------------------- | ------------------------------------------ |
| `VITE_API_BASE_URL`        | Backend API base URL | `https://api.example.com/api/v1/dashboard` |
| `VITE_MAP_STYLE_URL`       | Mapbox style URL     | `mapbox://styles/...`                      |
| `VITE_GOOGLE_MAPS_API_KEY` | Google Maps API key  | `AIza...`                                  |

### Preview Deployments

Every PR automatically gets a preview URL. No extra config needed — Vercel handles this natively.

### Custom Domain

1. Go to Project Settings → Domains
2. Add your domain
3. Update DNS as instructed by Vercel

## Manual Deployment

```bash
# Build
npm run build

# Preview locally
npm run preview

# Deploy dist/ to any static host
```

## CI Pipeline

GitHub Actions runs on every push/PR:

1. Install dependencies (`npm ci`)
2. Lint (`npm run lint`)
3. Type check (`npm run type-check`)
4. Test (`npm run test`)
5. Build (`npm run build`)

Build must pass before merge.
