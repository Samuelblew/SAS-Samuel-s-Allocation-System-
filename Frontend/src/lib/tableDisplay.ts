export function formatTableDate(value: string | null | undefined, locale: 'pt' | 'en'): string {
  if (!value) return '—'
  const date = new Date(`${value}T12:00:00`)
  if (Number.isNaN(date.getTime())) return value
  return new Intl.DateTimeFormat(locale === 'pt' ? 'pt-BR' : 'en-US', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
  }).format(date)
}

export type ToneLevel = 'low' | 'medium' | 'high' | 'critical' | 'neutral' | 'success'

export function priorityTone(priority: string): ToneLevel {
  switch (priority) {
    case 'Critical':
      return 'critical'
    case 'High':
      return 'high'
    case 'Medium':
      return 'medium'
    case 'Low':
      return 'low'
    default:
      return 'neutral'
  }
}

export function urgencyTone(urgency: string): ToneLevel {
  switch (urgency) {
    case 'High':
      return 'high'
    case 'Medium':
      return 'medium'
    case 'Low':
      return 'low'
    default:
      return 'neutral'
  }
}

export function dedicationTone(percent: number): ToneLevel {
  if (percent >= 80) return 'high'
  if (percent >= 50) return 'medium'
  return 'low'
}
