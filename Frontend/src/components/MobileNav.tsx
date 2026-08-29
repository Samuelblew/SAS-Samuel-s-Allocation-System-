import { useEffect } from 'react'
import { NavLink, useLocation } from 'react-router-dom'
import { LanguageSwitch } from './LanguageSwitch'
import { ThemeToggle } from './ThemeToggle'
import { useLocale } from '../context/LocaleContext'
import { useSettings } from '../context/SettingsContext'
import { useTheme } from '../context/ThemeContext'
import { getVisibleNavGroups, isNavItemActive } from '../lib/nav'
import { NavIcon } from './NavIcons'

export function MobileNav({ open, onClose }: { open: boolean; onClose: () => void }) {
  const { t, locale, setLocale } = useLocale()
  const { hiddenOptionsEnabled } = useSettings()
  const { theme, toggleTheme } = useTheme()
  const location = useLocation()
  const visibleNavGroups = getVisibleNavGroups(hiddenOptionsEnabled)

  useEffect(() => {
    if (!open) return
    const onKey = (e: KeyboardEvent) => {
      if (e.key === 'Escape') onClose()
    }
    document.addEventListener('keydown', onKey)
    document.body.style.overflow = 'hidden'
    return () => {
      document.removeEventListener('keydown', onKey)
      document.body.style.overflow = ''
    }
  }, [open, onClose])

  if (!open) return null

  return (
    <div className="ias-mobile-nav" role="dialog" aria-modal="true" aria-label={t('layout.navMobile')}>
      <button type="button" className="ias-mobile-nav-backdrop" aria-label={t('common.close')} onClick={onClose} />
      <div className="ias-mobile-nav-panel">
        <div className="ias-mobile-nav-header">
          <span className="ias-brand-name">{t('app.name')}</span>
          <button type="button" className="ias-mobile-nav-close" onClick={onClose} aria-label={t('common.close')}>
            ×
          </button>
        </div>
        <div className="ias-mobile-nav-body">
          {visibleNavGroups.map((group) => (
            <div key={group.labelKey} className="ias-mobile-nav-group">
              <p className="ias-mobile-nav-group-label">{t(group.labelKey)}</p>
              <ul className="space-y-1">
                {group.items.map((item) => (
                  <li key={item.to}>
                    <NavLink
                      to={item.to}
                      end={item.end}
                      onClick={onClose}
                      className={`ias-mobile-nav-link${
                        isNavItemActive(item, location.pathname) ? ' ias-mobile-nav-link-active' : ''
                      }`}
                    >
                      <NavIcon name={item.icon} className="h-4 w-4 shrink-0" />
                      {t(item.key)}
                    </NavLink>
                  </li>
                ))}
              </ul>
            </div>
          ))}
        </div>
        <div className="ias-mobile-nav-footer">
          <p className="ias-mobile-nav-footer-label">{t('layout.preferences')}</p>
          <div className="ias-mobile-nav-footer-controls">
            <LanguageSwitch locale={locale} onChange={setLocale} />
            <ThemeToggle theme={theme} onToggle={toggleTheme} />
          </div>
        </div>
      </div>
    </div>
  )
}
