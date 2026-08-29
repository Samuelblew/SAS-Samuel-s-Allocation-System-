import { useEffect, useRef, useState } from 'react'
import { NavLink, useLocation } from 'react-router-dom'
import { useLocale } from '../context/LocaleContext'
import type { NavGroup } from '../lib/nav'
import { isNavGroupActive, isNavItemActive } from '../lib/nav'
import { NavIcon } from './NavIcons'

const CLOSE_DELAY_MS = 120

function ChevronDown({ className }: { className?: string }) {
  return (
    <svg className={className} viewBox="0 0 12 12" fill="none" aria-hidden>
      <path
        d="M3 5l3 3 3-3"
        stroke="currentColor"
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  )
}

export function NavGroupMenu({ group }: { group: NavGroup }) {
  const { t } = useLocale()
  const location = useLocation()
  const [open, setOpen] = useState(false)
  const rootRef = useRef<HTMLDivElement>(null)
  const closeTimerRef = useRef<ReturnType<typeof setTimeout> | null>(null)
  const active = isNavGroupActive(group, location.pathname)
  const single = group.items.length === 1

  const clearCloseTimer = () => {
    if (closeTimerRef.current) {
      clearTimeout(closeTimerRef.current)
      closeTimerRef.current = null
    }
  }

  const scheduleClose = () => {
    clearCloseTimer()
    closeTimerRef.current = setTimeout(() => setOpen(false), CLOSE_DELAY_MS)
  }

  const handleOpen = () => {
    clearCloseTimer()
    setOpen(true)
  }

  useEffect(() => {
    return () => clearCloseTimer()
  }, [])

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

  if (single) {
    const item = group.items[0]
    return (
      <NavLink
        to={item.to}
        end={item.end}
        className={({ isActive }) =>
          `ias-header-nav-link${isActive ? ' ias-header-nav-link-active' : ''}`
        }
      >
        {t(item.key)}
      </NavLink>
    )
  }

  return (
    <div
      ref={rootRef}
      className="ias-nav-dropdown"
      onMouseEnter={handleOpen}
      onMouseLeave={scheduleClose}
      onFocus={handleOpen}
      onBlur={(e) => {
        if (!rootRef.current?.contains(e.relatedTarget as Node)) scheduleClose()
      }}
    >
      <button
        type="button"
        className={`ias-header-nav-link ias-header-nav-trigger${active ? ' ias-header-nav-link-active' : ''}`}
        aria-expanded={open}
        aria-haspopup="menu"
        onClick={() => setOpen((v) => !v)}
      >
        {t(group.labelKey)}
        <ChevronDown className="h-3 w-3 opacity-60" />
      </button>

      {open && (
        <div className="ias-nav-dropdown-flyout" onMouseEnter={handleOpen} onMouseLeave={scheduleClose}>
          <div className="ias-nav-dropdown-panel" role="menu">
            {group.items.map((item) => {
              const itemActive = isNavItemActive(item, location.pathname)
              return (
                <NavLink
                  key={item.to}
                  to={item.to}
                  end={item.end}
                  role="menuitem"
                  className={`ias-nav-dropdown-item${itemActive ? ' ias-nav-dropdown-item-active' : ''}`}
                  onClick={() => setOpen(false)}
                >
                  <NavIcon name={item.icon} className="h-4 w-4 shrink-0 opacity-90" />
                  <span>{t(item.key)}</span>
                </NavLink>
              )
            })}
          </div>
        </div>
      )}
    </div>
  )
}
