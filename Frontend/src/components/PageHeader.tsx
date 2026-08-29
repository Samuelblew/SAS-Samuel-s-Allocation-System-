import type { ReactNode } from 'react'

export function PageHeader({
  title,
  description,
  children,
  hideTitle,
}: {
  title?: string
  description?: string
  children?: ReactNode
  /** Quando true, o título já aparece na topbar do layout */
  hideTitle?: boolean
}) {
  const showTitle = title && !hideTitle
  const showDescription = description && !hideTitle

  if (!showTitle && !showDescription && !children) return null

  return (
    <div className="mb-6 flex flex-wrap items-start justify-between gap-4">
      <div className="min-w-0 flex-1">
        {showTitle && (
          <h2 className="ias-font-display text-xl font-semibold tracking-tight ias-text">{title}</h2>
        )}
        {showDescription && (
          <p
            className={`text-sm leading-relaxed ias-text-muted${showTitle ? ' mt-1.5' : ''}`}
          >
            {description}
          </p>
        )}
      </div>
      {children && <div className="flex shrink-0 flex-wrap items-center gap-3">{children}</div>}
    </div>
  )
}
