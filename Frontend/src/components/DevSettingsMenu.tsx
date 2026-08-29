import { useQuery } from '@tanstack/react-query'
import { useEffect, useRef, useState } from 'react'
import { useLocale } from '../context/LocaleContext'
import { useApiOptions, useSettings } from '../context/SettingsContext'
import { api } from '../lib/api'
import { inputClass } from '../lib/ui'
import { IconSettings } from './NavIcons'

interface HealthResponse {
  status: string
  service: string
}

export function DevSettingsMenu() {
  const { t } = useLocale()
  const {
    tenantId,
    actorId,
    hiddenOptionsEnabled,
    setTenantId,
    setActorId,
    setHiddenOptionsEnabled,
    isTenantValid,
  } = useSettings()
  const opts = useApiOptions()
  const [open, setOpen] = useState(false)
  const rootRef = useRef<HTMLDivElement>(null)

  const health = useQuery({
    queryKey: ['health'],
    queryFn: () => api.get<HealthResponse>('/api/v1/health', opts),
    enabled: open,
    retry: 1,
  })

  useEffect(() => {
    if (!open) return
    const onPointer = (e: MouseEvent) => {
      if (!rootRef.current?.contains(e.target as Node)) setOpen(false)
    }
    const onKey = (e: KeyboardEvent) => {
      if (e.key === 'Escape') setOpen(false)
    }
    document.addEventListener('click', onPointer)
    document.addEventListener('keydown', onKey)
    return () => {
      document.removeEventListener('click', onPointer)
      document.removeEventListener('keydown', onKey)
    }
  }, [open])

  const apiStatusLabel = health.isLoading
    ? t('layout.apiChecking')
    : health.isError
      ? t('layout.apiDown')
      : health.data?.status ?? '—'

  const apiPillClass =
    health.isSuccess && !health.isLoading
      ? ' ias-tenant-pill-ok'
      : health.isError
        ? ' ias-tenant-pill-warn'
        : ''

  return (
    <div ref={rootRef} className="ias-nav-dropdown">
      <button
        type="button"
        className="ias-header-icon-btn"
        aria-expanded={open}
        aria-label={t('layout.devConfig')}
        onClick={() => setOpen((v) => !v)}
      >
        <IconSettings className="h-[18px] w-[18px]" />
      </button>

      {open && (
        <div className="ias-dev-settings-flyout">
          <div className="ias-dev-settings-panel">
            <p className="ias-dev-settings-title">{t('layout.devConfig')}</p>
            <div className={`ias-tenant-pill mb-3${isTenantValid ? ' ias-tenant-pill-ok' : ''}`}>
              <span className="ias-tenant-dot" aria-hidden />
              <span className="truncate text-xs font-medium">
                {isTenantValid ? t('layout.tenantReady') : t('layout.tenantPending')}
              </span>
            </div>
            <div className={`ias-tenant-pill mb-3${apiPillClass}`}>
              <span className="ias-tenant-dot" aria-hidden />
              <span className="truncate text-xs font-medium">
                {t('layout.api')}: {apiStatusLabel}
              </span>
            </div>
            <label className="mt-3 flex items-center justify-between gap-3">
              <span className="ias-label">{t('layout.hiddenOptions')}</span>
              <span className="ias-switch">
                <input
                  type="checkbox"
                  className="ias-switch-input"
                  checked={hiddenOptionsEnabled}
                  onChange={(e) => setHiddenOptionsEnabled(e.target.checked)}
                />
                <span className="ias-switch-track" aria-hidden />
              </span>
            </label>
            <label className="mt-3 flex flex-col gap-1.5">
              <span className="ias-label">{t('layout.tenant')}</span>
              <input
                type="text"
                value={tenantId}
                onChange={(e) => setTenantId(e.target.value.trim())}
                placeholder={t('layout.tenantPlaceholder')}
                className={inputClass}
              />
            </label>
            <label className="mt-3 flex flex-col gap-1.5">
              <span className="ias-label">{t('layout.actor')}</span>
              <input
                type="text"
                value={actorId}
                onChange={(e) => setActorId(e.target.value.trim())}
                placeholder={t('layout.actorPlaceholder')}
                className={inputClass}
              />
            </label>
          </div>
        </div>
      )}
    </div>
  )
}
