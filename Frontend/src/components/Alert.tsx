export function Alert({ message, variant = 'error' }: { message: string; variant?: 'error' | 'info' | 'success' | 'warning' }) {
  const styles = {
    error: 'ias-alert-error',
    info: 'ias-alert-info',
    success: 'ias-alert-success',
    warning: 'ias-alert-warning',
  }[variant]

  return <div className={`rounded-lg px-4 py-3 text-sm ${styles}`}>{message}</div>
}
