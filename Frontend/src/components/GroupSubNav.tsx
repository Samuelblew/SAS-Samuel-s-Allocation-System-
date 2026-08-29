import { NavLink, useLocation } from 'react-router-dom'
import { useLocale } from '../context/LocaleContext'
import type { NavGroup } from '../lib/nav'
import { isNavItemActive } from '../lib/nav'

export function GroupSubNav({ group }: { group: NavGroup }) {
  const { t } = useLocale()
  const location = useLocation()

  if (group.items.length <= 1) return null

  return (
    <nav className="ias-workflow-stepper" aria-label={t(group.labelKey)}>
      <ol className="ias-workflow-stepper__list">
        {group.items.map((item, index) => {
          const active = isNavItemActive(item, location.pathname)
          const num = String(index + 1).padStart(2, '0')
          const isLast = index === group.items.length - 1

          return (
            <li
              key={item.to}
              className={`ias-workflow-stepper__item${active ? ' ias-workflow-stepper__item--active' : ''}${isLast ? ' ias-workflow-stepper__item--last' : ''}`}
            >
              <NavLink to={item.to} end={item.end} className="ias-workflow-stepper__link">
                <span className="ias-workflow-stepper__index">{num}</span>
                <span className="ias-workflow-stepper__label">{t(item.key)}</span>
              </NavLink>
              {!isLast && <span className="ias-workflow-stepper__rail" aria-hidden />}
            </li>
          )
        })}
      </ol>
    </nav>
  )
}
