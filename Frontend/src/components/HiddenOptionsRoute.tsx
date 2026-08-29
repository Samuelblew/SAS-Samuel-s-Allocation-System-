import { Navigate } from 'react-router-dom'
import type { ReactNode } from 'react'
import { useSettings } from '../context/SettingsContext'
import { isHiddenNavPath } from '../lib/nav'

export function HiddenOptionsRoute({
  path,
  children,
}: {
  path: string
  children: ReactNode
}) {
  const { hiddenOptionsEnabled } = useSettings()

  if (!hiddenOptionsEnabled && isHiddenNavPath(path)) {
    return <Navigate to="/" replace />
  }

  return children
}
