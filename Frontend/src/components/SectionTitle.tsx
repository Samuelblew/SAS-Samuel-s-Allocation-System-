import type { ReactNode } from 'react'

export function SectionTitle({ children }: { children: ReactNode }) {
  return <h3 className="ias-section-title">{children}</h3>
}
