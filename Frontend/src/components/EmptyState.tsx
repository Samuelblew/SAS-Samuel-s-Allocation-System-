import { Card } from './Card'

export function EmptyState({
  message,
  variant = 'muted',
}: {
  message: string
  variant?: 'muted' | 'success'
}) {
  return (
    <Card>
      <p className={`text-sm ${variant === 'success' ? 'ias-alert-success rounded-lg px-3 py-2 inline-block' : 'ias-text-muted'}`}>
        {message}
      </p>
    </Card>
  )
}
