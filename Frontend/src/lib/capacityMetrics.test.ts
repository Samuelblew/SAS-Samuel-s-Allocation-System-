import { describe, expect, it } from 'vitest'
import {
  donutSlicesFromWeek,
  hoursSplitFromWeek,
  periodAverage,
  utilizationTone,
} from './capacityMetrics'
import type { WeekOverview } from './types'

const sampleWeek: WeekOverview = {
  weekStart: '2026-06-02',
  weekEnd: '2026-06-08',
  activePeopleCount: 4,
  avgAllocatedPercent: 60,
  avgAvailablePercent: 30,
  benchPeopleCount: 1,
  overallocatedPeopleCount: 0,
  totalCapacityHours: 160,
  totalAllocatedHours: 96,
  totalAvailableHours: 32,
}

describe('capacityMetrics', () => {
  it('calculates unavailable hours from week totals', () => {
    const split = hoursSplitFromWeek(sampleWeek)
    expect(split.unavailableHours).toBe(32)
    expect(split.allocatedHours + split.availableHours + split.unavailableHours).toBe(160)
  })

  it('builds donut slices from hours', () => {
    const slices = donutSlicesFromWeek(sampleWeek)
    expect(slices).toHaveLength(3)
    expect(slices.find((s) => s.id === 'allocated')?.percent).toBe(60)
    expect(slices.find((s) => s.id === 'available')?.percent).toBe(20)
    expect(slices.find((s) => s.id === 'unavailable')?.percent).toBe(20)
  })

  it('averages weekly metrics', () => {
    const avg = periodAverage(
      [
        { ...sampleWeek, avgAllocatedPercent: 40 },
        { ...sampleWeek, avgAllocatedPercent: 60 },
      ],
      (w) => w.avgAllocatedPercent,
    )
    expect(avg).toBe(50)
  })

  it('classifies utilization tone', () => {
    expect(utilizationTone(30)).toBe('low')
    expect(utilizationTone(70)).toBe('balanced')
    expect(utilizationTone(90)).toBe('high')
  })
})
