import type { WeekCapacityGap, WeekOverview } from './types'

export interface CapacityHoursSplit {
  totalCapacityHours: number
  allocatedHours: number
  availableHours: number
  unavailableHours: number
}

export interface CapacitySlice {
  id: 'allocated' | 'available' | 'unavailable'
  value: number
  percent: number
}

export function periodAverage(
  weeks: WeekOverview[],
  pick: (week: WeekOverview) => number,
): number | null {
  if (weeks.length === 0) return null
  return weeks.reduce((sum, week) => sum + pick(week), 0) / weeks.length
}

export function lastWeekOverview(weeks: WeekOverview[]): WeekOverview | null {
  return weeks.length > 0 ? weeks[weeks.length - 1]! : null
}

export function hoursSplitFromWeek(week: WeekOverview): CapacityHoursSplit {
  const unavailableHours = Math.max(
    0,
    week.totalCapacityHours - week.totalAllocatedHours - week.totalAvailableHours,
  )
  return {
    totalCapacityHours: week.totalCapacityHours,
    allocatedHours: week.totalAllocatedHours,
    availableHours: week.totalAvailableHours,
    unavailableHours,
  }
}

export function donutSlicesFromWeek(week: WeekOverview): CapacitySlice[] {
  const split = hoursSplitFromWeek(week)
  const total = split.totalCapacityHours
  if (total <= 0) return []

  const toPercent = (hours: number) => Math.round((hours / total) * 1000) / 10

  return [
    { id: 'allocated' as const, value: split.allocatedHours, percent: toPercent(split.allocatedHours) },
    { id: 'available' as const, value: split.availableHours, percent: toPercent(split.availableHours) },
    {
      id: 'unavailable' as const,
      value: split.unavailableHours,
      percent: toPercent(split.unavailableHours),
    },
  ].filter((slice) => slice.value > 0)
}

export function formatWeekAxisLabel(weekStart: string, locale: string): string {
  const [year, month, day] = weekStart.split('-').map(Number)
  const date = new Date(year, month - 1, day)
  return date.toLocaleDateString(locale === 'en' ? 'en-US' : 'pt-BR', {
    day: 'numeric',
    month: 'short',
  })
}

export function peakGapWeek(weeks: WeekCapacityGap[]): WeekCapacityGap | null {
  if (weeks.length === 0) return null
  return weeks.reduce((peak, week) =>
    week.netShortfallPercent > peak.netShortfallPercent ? week : peak,
  )
}

export function utilizationTone(percent: number | null): 'low' | 'balanced' | 'high' {
  if (percent == null) return 'balanced'
  if (percent < 50) return 'low'
  if (percent > 85) return 'high'
  return 'balanced'
}
