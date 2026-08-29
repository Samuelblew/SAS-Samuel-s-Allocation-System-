import { Card } from './Card'

export type StatCardHighlight = 'success' | 'warning' | false

export function StatCard({
  label,
  value,
  loading,
  highlight = false,
}: {
  label: string
  value?: number | string
  loading?: boolean
  highlight?: StatCardHighlight
}) {
  const cardClass =
    highlight === 'success'
      ? 'ias-border-success'
      : highlight === 'warning'
        ? 'border-[var(--ias-warning-border)]'
        : ''

  const valueClass =
    highlight === 'success'
      ? 'ias-text-success'
      : highlight === 'warning'
        ? 'ias-text-warning'
        : 'ias-text'

  return (
    <Card className={cardClass}>
      <p className="ias-stat-label">{label}</p>
      <p className={`ias-stat-value ${valueClass}`}>
        {loading ? '—' : (value ?? '—')}
      </p>
    </Card>
  )
}
