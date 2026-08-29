import type { ReactNode } from 'react'
import { labelClass } from '../../lib/ui'

export function FormField({
  label,
  error,
  className = '',
  children,
}: {
  label: string
  error?: string
  className?: string
  children: ReactNode
}) {
  return (
    <label className={`${labelClass} ${className}`.trim()}>
      {label}
      {children}
      {error && <span className="text-xs ias-text-danger">{error}</span>}
    </label>
  )
}
