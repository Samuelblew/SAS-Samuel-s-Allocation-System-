import type { AllocationListItem, AllocationStatus } from './types'

export interface AllocationLoadItem {
  id: string
  projectName: string
  dedicationPercent: number
  startDate: string
  endDate: string
  status: AllocationStatus
}

export interface WeekOverloadDetail {
  weekStart: string
  weekEnd: string
  requestedPercent: number
  existingPercent: number
  totalPercent: number
  existing: AllocationLoadItem[]
}

function parseIsoDate(value: string): Date {
  const [year, month, day] = value.split('-').map(Number)
  return new Date(year, month - 1, day)
}

function formatIsoDate(date: Date): string {
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}

function startOfWeek(date: Date): Date {
  const result = new Date(date)
  const offset = (result.getDay() + 6) % 7
  result.setDate(result.getDate() - offset)
  return result
}

export function enumerateWeeks(startIso: string, endIso: string): { start: string; end: string }[] {
  const start = parseIsoDate(startIso)
  const end = parseIsoDate(endIso)
  const weeks: { start: string; end: string }[] = []

  let cursor = startOfWeek(start)
  const last = startOfWeek(end)

  while (cursor <= last) {
    const weekEnd = new Date(cursor)
    weekEnd.setDate(weekEnd.getDate() + 6)
    weeks.push({
      start: formatIsoDate(cursor),
      end: formatIsoDate(weekEnd),
    })
    cursor = new Date(cursor)
    cursor.setDate(cursor.getDate() + 7)
  }

  return weeks
}

function overlaps(
  aStart: string,
  aEnd: string,
  bStart: string,
  bEnd: string,
): boolean {
  return aStart <= bEnd && aEnd >= bStart
}

export function toAllocationLoadItem(item: AllocationListItem): AllocationLoadItem {
  return {
    id: item.id,
    projectName: item.projectName,
    dedicationPercent: item.dedicationPercent,
    startDate: item.startDate,
    endDate: item.endDate,
    status: item.status,
  }
}

export function filterActiveOverlapping(
  allocations: AllocationLoadItem[],
  startIso: string,
  endIso: string,
  excludeAllocationId?: string | null,
): AllocationLoadItem[] {
  return allocations.filter(
    (allocation) =>
      allocation.status !== 'Closed' &&
      allocation.id !== excludeAllocationId &&
      overlaps(allocation.startDate, allocation.endDate, startIso, endIso),
  )
}

export function findFirstWeeklyOverload(
  startIso: string,
  endIso: string,
  dedicationPercent: number,
  allocations: AllocationLoadItem[],
  excludeAllocationId?: string | null,
): WeekOverloadDetail | null {
  if (!startIso || !endIso || dedicationPercent <= 0) return null

  for (const week of enumerateWeeks(startIso, endIso)) {
    const inWeek = allocations.filter(
      (allocation) =>
        allocation.status !== 'Closed' &&
        allocation.id !== excludeAllocationId &&
        overlaps(allocation.startDate, allocation.endDate, week.start, week.end),
    )

    const existingPercent = inWeek.reduce((sum, allocation) => sum + allocation.dedicationPercent, 0)
    const totalPercent = dedicationPercent + existingPercent

    if (totalPercent > 100) {
      return {
        weekStart: week.start,
        weekEnd: week.end,
        requestedPercent: dedicationPercent,
        existingPercent,
        totalPercent,
        existing: inWeek,
      }
    }
  }

  return null
}

export function peakWeeklyLoad(
  startIso: string,
  endIso: string,
  allocations: AllocationLoadItem[],
  excludeAllocationId?: string | null,
): { weekStart: string; weekEnd: string; allocatedPercent: number } | null {
  const weeks = enumerateWeeks(startIso, endIso)
  if (weeks.length === 0) return null

  let peak: { weekStart: string; weekEnd: string; allocatedPercent: number } | null = null

  for (const week of weeks) {
    const allocatedPercent = allocations
      .filter(
        (allocation) =>
          allocation.status !== 'Closed' &&
          allocation.id !== excludeAllocationId &&
          overlaps(allocation.startDate, allocation.endDate, week.start, week.end),
      )
      .reduce((sum, allocation) => sum + allocation.dedicationPercent, 0)

    if (!peak || allocatedPercent > peak.allocatedPercent) {
      peak = { weekStart: week.start, weekEnd: week.end, allocatedPercent }
    }
  }

  return peak
}
