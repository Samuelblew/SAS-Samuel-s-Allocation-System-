import type { ReactNode } from 'react'

export function WorkspacePanel({
  title,
  meta,
  children,
  flush,
}: {
  title?: string
  meta?: ReactNode
  children: ReactNode
  flush?: boolean
}) {
  return (
    <section className={`ias-workspace-panel${flush ? ' ias-workspace-panel--flush' : ''}`}>
      {(title || meta) && (
        <header className="ias-workspace-panel__header">
          {title ? <h2 className="ias-workspace-panel__title">{title}</h2> : <span />}
          {meta}
        </header>
      )}
      <div className="ias-workspace-panel__body">{children}</div>
    </section>
  )
}
