export const CAPACITY_CHART_COLORS = {
  allocated: '#0066ff',
  available: '#33cccc',
  unavailable: 'rgba(13, 24, 63, 0.28)',
  demand: '#0033cc',
  supply: '#33cccc',
  overload: '#dc2626',
} as const

export const CHART_TOOLTIP_STYLE = {
  backgroundColor: 'var(--ias-surface)',
  border: '1px solid var(--ias-border-strong)',
  borderRadius: '10px',
  color: 'var(--ias-text)',
  fontSize: '12px',
} as const
