import type { ReactNode } from 'react'

export type FormGridSpan = 1 | 2 | 3 | 4 | 5 | 6

export function FormGrid({ children }: { children: ReactNode }) {
  return <div className="ias-form-grid">{children}</div>
}

export function FormGridField({
  span = 1,
  children,
}: {
  span?: FormGridSpan
  children: ReactNode
}) {
  return (
    <div className="ias-form-grid__field" style={{ gridColumn: `span ${span}` }}>
      {children}
    </div>
  )
}
