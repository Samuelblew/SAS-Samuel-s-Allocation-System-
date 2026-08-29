import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import { getDomainLabels } from '../i18n/domain'
import { translatePage, type PageMessageKey } from '../i18n/pageMessages'
import { translate, type Locale, type MessageKey } from '../i18n/messages'

const STORAGE_KEY = 'ias.locale'

interface LocaleContextValue {
  locale: Locale
  setLocale: (locale: Locale) => void
  t: (key: MessageKey, vars?: Record<string, string>) => string
  tp: (key: PageMessageKey, vars?: Record<string, string | number>) => string
  domain: ReturnType<typeof getDomainLabels>
}

const LocaleContext = createContext<LocaleContextValue | null>(null)

function readLocale(): Locale {
  try {
    const stored = localStorage.getItem(STORAGE_KEY)
    if (stored === 'pt' || stored === 'en') return stored
  } catch {
    /* ignore */
  }
  return 'pt'
}

export function LocaleProvider({ children }: { children: ReactNode }) {
  const [locale, setLocaleState] = useState<Locale>(() => readLocale())

  const setLocale = useCallback((value: Locale) => {
    setLocaleState(value)
    try {
      localStorage.setItem(STORAGE_KEY, value)
    } catch {
      /* ignore */
    }
  }, [])

  const value = useMemo(
    () => ({
      locale,
      setLocale,
      t: (key: MessageKey, vars?: Record<string, string>) => translate(locale, key, vars),
      tp: (key: PageMessageKey, vars?: Record<string, string | number>) =>
        translatePage(locale, key, vars),
      domain: getDomainLabels(locale),
    }),
    [locale, setLocale],
  )

  return <LocaleContext.Provider value={value}>{children}</LocaleContext.Provider>
}

export function useLocale() {
  const ctx = useContext(LocaleContext)
  if (!ctx) throw new Error('useLocale deve ser usado dentro de LocaleProvider')
  return ctx
}

export function useDomainLabels() {
  return useLocale().domain
}
