import type { ToneLevel } from '../lib/tableDisplay'

const toneClass: Record<ToneLevel, string> = {
  low: 'ias-tone-badge--low',
  medium: 'ias-tone-badge--medium',
  high: 'ias-tone-badge--high',
  critical: 'ias-tone-badge--critical',
  neutral: 'ias-tone-badge--neutral',
  success: 'ias-tone-badge--success',
}

export function ToneBadge({ label, tone = 'neutral' }: { label: string; tone?: ToneLevel }) {
  return <span className={`ias-tone-badge ${toneClass[tone]}`}>{label}</span>
}
