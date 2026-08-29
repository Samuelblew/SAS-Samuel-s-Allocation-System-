import { useState } from 'react'
import { Link, Outlet, useLocation } from 'react-router-dom'
import { DevSettingsMenu } from './DevSettingsMenu'
import { GroupSubNav } from './GroupSubNav'
import { LanguageSwitch } from './LanguageSwitch'
import { MobileNav } from './MobileNav'
import { NavGroupMenu } from './NavGroupMenu'
import { ThemeToggle } from './ThemeToggle'
import { useLocale } from '../context/LocaleContext'
import { useSettings } from '../context/SettingsContext'
import { useTheme } from '../context/ThemeContext'
import {
  getActiveNavGroup,
  getGroupKey,
  getPageDescriptionKey,
  getPageTitleKey,
  getVisibleNavGroups,
} from '../lib/nav'

function BrandMark() {
  return <img src="/brand/mark.png" alt="" className="ias-header-brand-logo" aria-hidden />
}

function MenuIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden className="h-6 w-6 stroke-current fill-none" strokeWidth="2">
      <path d="M3 6h18" strokeLinecap="round" />
      <path d="M3 12h18" strokeLinecap="round" />
      <path d="M3 18h18" strokeLinecap="round" />
    </svg>
  )
}

export function Layout() {
  const { isTenantValid, hiddenOptionsEnabled } = useSettings()
  const { locale, setLocale, t } = useLocale()
  const { theme, toggleTheme } = useTheme()
  const location = useLocation()
  const [mobileOpen, setMobileOpen] = useState(false)

  const pageTitle = t(getPageTitleKey(location.pathname))
  const groupLabel = t(getGroupKey(location.pathname))
  const descriptionKey = getPageDescriptionKey(location.pathname)
  const description = descriptionKey ? t(descriptionKey) : undefined
  const activeGroup = getActiveNavGroup(location.pathname, hiddenOptionsEnabled)
  const visibleNavGroups = getVisibleNavGroups(hiddenOptionsEnabled)

  return (
    <div className="ias-shell">
      <div className="ias-app-frame">
        <header className="ias-header-pill">
          <Link to="/" className="ias-header-brand">
            <BrandMark />
            <span className="ias-brand-name ias-header-brand-name">{t('app.name')}</span>
          </Link>

          <nav className="ias-header-nav" aria-label={t('layout.navMobile')}>
            {visibleNavGroups.map((group) => (
              <NavGroupMenu key={group.labelKey} group={group} />
            ))}
          </nav>

          <div className="ias-header-actions">
            <div className="ias-header-utilities">
              <div
                className={`ias-tenant-chip${isTenantValid ? ' ias-tenant-chip-ok' : ''}`}
                title={isTenantValid ? t('layout.tenantReady') : t('layout.tenantPending')}
              >
                <span className="ias-tenant-dot" aria-hidden />
              </div>
              <LanguageSwitch locale={locale} onChange={setLocale} />
              <ThemeToggle theme={theme} onToggle={toggleTheme} />
              <DevSettingsMenu />
            </div>

            <div className="ias-header-mobile-controls">
              <ThemeToggle theme={theme} onToggle={toggleTheme} variant="icon" />
              <DevSettingsMenu />
              <button
                type="button"
                className="ias-header-icon-btn"
                aria-label={t('layout.menu')}
                aria-expanded={mobileOpen}
                onClick={() => setMobileOpen(true)}
              >
                <MenuIcon />
              </button>
            </div>
          </div>
        </header>

        <MobileNav open={mobileOpen} onClose={() => setMobileOpen(false)} />

        {!isTenantValid && (
          <div className="ias-frame-alert">
            <p className="ias-alert-warning rounded-full px-4 py-2 text-sm">{t('layout.tenantInvalid')}</p>
          </div>
        )}

        <div className="ias-frame-body">
          <div className="ias-frame-toolbar">
            <section className="ias-page-chrome">
              <p className="ias-breadcrumb">
                {t('layout.workspace')} · {groupLabel}
              </p>
              <h1 className="ias-page-title">{pageTitle}</h1>
              {description && <p className="ias-page-lead">{description}</p>}
            </section>
          </div>

          <main className="ias-page-content">
            <GroupSubNav group={activeGroup} />
            <div className="ias-page-content-inner">
              <Outlet />
            </div>
          </main>
        </div>
      </div>
    </div>
  )
}
