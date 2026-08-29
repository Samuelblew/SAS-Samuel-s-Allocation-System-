import type { ReactNode } from 'react'

export type PageHeroTone = 'low' | 'balanced' | 'high' | 'success' | 'warning'

export function PageHero({
  label,
  value,
  hint,
  alert,
  tone = 'balanced',
  metrics,
}: {
  label: string
  value: ReactNode
  hint?: string
  alert?: string
  tone?: PageHeroTone
  metrics?: ReactNode
}) {
  return (
    <section className="ias-page-hero mb-4">
      <div className="ias-page-hero__main">
        <p className="ias-page-hero__label">{label}</p>
        <p className={`ias-page-hero__value ias-page-hero__value--${tone}`}>{value}</p>
        {hint ? <p className="ias-page-hero__hint">{hint}</p> : null}
        {alert ? <p className="ias-page-hero__alert">{alert}</p> : null}
      </div>
      {metrics ? <div className="ias-page-hero__metrics">{metrics}</div> : null}
    </section>
  )
}

export function PageHeroMetric({
  label,
  value,
  valueClassName = '',
}: {
  label: string
  value: ReactNode
  valueClassName?: string
}) {
  return (
    <div className="ias-page-mini-metric">
      <span className="ias-page-mini-metric__label">{label}</span>
      <span className={`ias-page-mini-metric__value ${valueClassName}`}>{value}</span>
    </div>
  )
}
