const isDev = import.meta.env.DEV

export const logger = {
  log: (...args: unknown[]): void => {
    if (isDev) console.log(...args)
  },
  error: (...args: unknown[]): void => {
    if (isDev) console.error(...args)
  },
  warn: (...args: unknown[]): void => {
    if (isDev) console.warn(...args)
  },
}
