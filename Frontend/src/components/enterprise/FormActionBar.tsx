import type { ReactNode } from 'react'

export function FormActionBar({
  leading,
  children,
}: {
  leading?: ReactNode
  children: ReactNode
}) {
  return (
    <div className="ias-form-action-bar">
      {leading ? <div className="ias-form-action-bar__leading">{leading}</div> : <span />}
      <div className="ias-form-action-bar__actions">{children}</div>
    </div>
  )
}
