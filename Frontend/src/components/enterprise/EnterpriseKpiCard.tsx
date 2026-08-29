import type { ReactNode } from 'react'

export function EnterpriseKpiCard({
  label,
  value,
  description,
  loading,
}: {
  label: string
  value: ReactNode
  description: string
  loading?: boolean
}) {
  return (
    <article className="ias-enterprise-kpi">
      <p className="ias-enterprise-kpi__label">{label}</p>
      <p className="ias-enterprise-kpi__value">{loading ? '—' : value}</p>
      <p className="ias-enterprise-kpi__desc">{description}</p>
    </article>
  )
}

export function EnterpriseKpiStrip({ children }: { children: ReactNode }) {
  return <div className="ias-enterprise-kpi-strip">{children}</div>
}
