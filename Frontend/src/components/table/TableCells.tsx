import type { ReactNode } from 'react'
import { useLocale } from '../../context/LocaleContext'
import { formatTableDate, type ToneLevel } from '../../lib/tableDisplay'
import { ToneBadge } from '../ToneBadge'

export function TableCellIdentity({
  title,
  subtitle,
}: {
  title: string
  subtitle?: string | null
}) {
  return (
    <div className="ias-table-identity">
      <p className="ias-table-identity-title truncate">{title}</p>
      {subtitle ? <p className="ias-table-identity-sub truncate">{subtitle}</p> : null}
    </div>
  )
}

export function TableCellChip({
  label,
  emptyLabel,
  tone = 'neutral',
}: {
  label?: string | null
  emptyLabel?: string
  tone?: ToneLevel
}) {
  const { t } = useLocale()
  const text = label?.trim() || emptyLabel || t('common.none')
  const isEmpty = !label?.trim()

  if (isEmpty) {
    return <span className="ias-table-empty-value">{text}</span>
  }

  return <ToneBadge label={text} tone={tone} />
}

export function TableCellMetric({
  value,
  unit,
  tone = 'neutral',
  showBar,
  barMax = 100,
}: {
  value: number | string
  unit?: string
  tone?: ToneLevel
  showBar?: boolean
  barMax?: number
}) {
  const numeric = typeof value === 'number' ? value : Number.parseFloat(String(value))
  const barPercent =
    showBar && Number.isFinite(numeric) ? Math.min(100, Math.max(0, (numeric / barMax) * 100)) : 0

  return (
    <div className="ias-table-metric">
      <p className={`ias-table-metric-value ias-table-metric-value--${tone}`}>
        {value}
        {unit ? <span className="ias-table-metric-unit">{unit}</span> : null}
      </p>
      {showBar && Number.isFinite(numeric) ? (
        <div className="ias-table-metric-bar" aria-hidden>
          <div
            className={`ias-table-metric-bar-fill ias-table-metric-bar-fill--${tone}`}
            style={{ width: `${barPercent}%` }}
          />
        </div>
      ) : null}
    </div>
  )
}

export function TableCellPeriod({
  start,
  end,
  emptyLabel,
}: {
  start?: string | null
  end?: string | null
  emptyLabel?: string
}) {
  const { locale, t } = useLocale()
  const none = emptyLabel ?? t('common.none')
  const startLabel = start ? formatTableDate(start, locale) : none
  const endLabel = end ? formatTableDate(end, locale) : none

  if (!start && !end) {
    return <span className="ias-table-empty-value">{none}</span>
  }

  return (
    <div className="ias-table-period">
      <span className="ias-table-period-date">{startLabel}</span>
      <span className="ias-table-period-arrow" aria-hidden>
        →
      </span>
      <span className="ias-table-period-date">{endLabel}</span>
    </div>
  )
}

export function TableCellSkillCount({ count }: { count: number }) {
  const { t } = useLocale()

  if (count <= 0) {
    return <span className="ias-table-empty-value">{t('form.people.noSkills')}</span>
  }

  return (
    <span className="ias-table-skill-count">
      {count} {count === 1 ? t('table.skillSingular') : t('table.skillPlural')}
    </span>
  )
}

export function TableCellStack({ children }: { children: ReactNode }) {
  return <div className="ias-table-cell-stack">{children}</div>
}
