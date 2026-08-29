import type { Theme } from '../context/ThemeContext'
import { useLocale } from '../context/LocaleContext'
import { IconMoon, IconSun } from './NavIcons'

export function ThemeToggle({
  theme,
  onToggle,
  variant = 'default',
}: {
  theme: Theme
  onToggle: () => void
  variant?: 'default' | 'icon'
}) {
  const { t } = useLocale()
  const isDark = theme === 'dark'
  const label = isDark ? t('layout.themeLight') : t('layout.themeDark')

  if (variant === 'icon') {
    return (
      <button
        type="button"
        onClick={onToggle}
        className="ias-header-icon-btn"
        aria-label={label}
        title={label}
      >
        {isDark ? <IconSun className="h-[18px] w-[18px]" /> : <IconMoon className="h-[18px] w-[18px]" />}
      </button>
    )
  }

  return (
    <button
      type="button"
      onClick={onToggle}
      className="ias-btn-ghost flex items-center gap-2 px-2 py-1.5 text-xs font-medium sm:px-3"
      aria-label={label}
      title={label}
    >
      {isDark ? <IconSun className="h-4 w-4 shrink-0" /> : <IconMoon className="h-4 w-4 shrink-0" />}
      <span className="hidden sm:inline">{label}</span>
    </button>
  )
}
