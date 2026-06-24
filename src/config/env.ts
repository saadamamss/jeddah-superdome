/**
 * Environment Configuration
 * Validates and exports all environment variables with proper types.
 * All env vars must be prefixed with VITE_ to be exposed to the client.
 */

function getEnvVar(key: string, required = true): string {
  const value = import.meta.env[key]
  if (required && !value) {
    throw new Error(`Missing required environment variable: ${key}`)
  }
  return value ?? ''
}

export const env = {
  API_BASE_URL: getEnvVar('VITE_API_BASE_URL'),
  MAP_STYLE_URL: getEnvVar('VITE_MAP_STYLE_URL', false),
  GOOGLE_MAPS_API_KEY: getEnvVar('VITE_GOOGLE_MAPS_API_KEY', false),
} as const
