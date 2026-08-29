import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState,
  type ReactNode,
} from 'react'

const TENANT_KEY = 'ias.tenantId'
const ACTOR_KEY = 'ias.actorId'
const HIDDEN_OPTIONS_KEY = 'ias.hiddenOptions'

interface SettingsContextValue {
  tenantId: string
  actorId: string
  hiddenOptionsEnabled: boolean
  setTenantId: (value: string) => void
  setActorId: (value: string) => void
  setHiddenOptionsEnabled: (value: boolean) => void
  isTenantValid: boolean
}

const SettingsContext = createContext<SettingsContextValue | null>(null)

function readStorage(key: string): string {
  try {
    return localStorage.getItem(key) ?? ''
  } catch {
    return ''
  }
}

function readBoolean(key: string): boolean {
  try {
    return localStorage.getItem(key) === 'true'
  } catch {
    return false
  }
}

function isGuid(value: string): boolean {
  return /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(value)
}

export function SettingsProvider({ children }: { children: ReactNode }) {
  const [tenantId, setTenantIdState] = useState(() => readStorage(TENANT_KEY))
  const [actorId, setActorIdState] = useState(() => readStorage(ACTOR_KEY))
  const [hiddenOptionsEnabled, setHiddenOptionsEnabledState] = useState(() =>
    readBoolean(HIDDEN_OPTIONS_KEY),
  )

  const setTenantId = useCallback((value: string) => {
    setTenantIdState(value)
    try {
      localStorage.setItem(TENANT_KEY, value)
    } catch {
      /* ignore */
    }
  }, [])

  const setActorId = useCallback((value: string) => {
    setActorIdState(value)
    try {
      localStorage.setItem(ACTOR_KEY, value)
    } catch {
      /* ignore */
    }
  }, [])

  const setHiddenOptionsEnabled = useCallback((value: boolean) => {
    setHiddenOptionsEnabledState(value)
    try {
      localStorage.setItem(HIDDEN_OPTIONS_KEY, value ? 'true' : 'false')
    } catch {
      /* ignore */
    }
  }, [])

  const value = useMemo(
    () => ({
      tenantId,
      actorId,
      hiddenOptionsEnabled,
      setTenantId,
      setActorId,
      setHiddenOptionsEnabled,
      isTenantValid: isGuid(tenantId),
    }),
    [tenantId, actorId, hiddenOptionsEnabled, setTenantId, setActorId, setHiddenOptionsEnabled],
  )

  return <SettingsContext.Provider value={value}>{children}</SettingsContext.Provider>
}

export function useSettings() {
  const ctx = useContext(SettingsContext)
  if (!ctx) {
    throw new Error('useSettings deve ser usado dentro de SettingsProvider')
  }
  return ctx
}

export function useApiOptions() {
  const { tenantId, actorId } = useSettings()
  return { tenantId, actorId: actorId || undefined }
}
