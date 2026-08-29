import type { ReactNode } from 'react'
import { Card } from '../Card'
import { InfoTip } from '../InfoTip'
import { SectionTitle } from '../SectionTitle'

export function PageSection({
  title,
  titleHint,
  subtitle,
  children,
  className = '',
}: {
  title: string
  titleHint?: string
  subtitle?: string
  children: ReactNode
  className?: string
}) {
  return (
    <Card className={`${className}`.trim()}>
      <div className="ias-section-title-row">
        <SectionTitle>{title}</SectionTitle>
        {titleHint ? <InfoTip text={titleHint} /> : null}
      </div>
      {subtitle ? <p className="ias-page-card-subtitle">{subtitle}</p> : null}
      {children}
    </Card>
  )
}
