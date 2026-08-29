import { describe, expect, it } from 'vitest'
import { findFirstWeeklyOverload, peakWeeklyLoad } from './allocationLoad'
import type { AllocationLoadItem } from './allocationLoad'

function allocation(
  overrides: Partial<AllocationLoadItem> & Pick<AllocationLoadItem, 'dedicationPercent'>,
): AllocationLoadItem {
  return {
    id: overrides.id ?? 'a1',
    projectName: overrides.projectName ?? 'Projeto A',
    dedicationPercent: overrides.dedicationPercent,
    startDate: overrides.startDate ?? '2026-06-01',
    endDate: overrides.endDate ?? '2026-06-30',
    status: overrides.status ?? 'Confirmed',
  }
}

describe('findFirstWeeklyOverload', () => {
  it('retorna null sem alocações existentes para 50%', () => {
    const result = findFirstWeeklyOverload('2026-06-01', '2026-06-30', 50, [])
    expect(result).toBeNull()
  })

  it('detecta soma acima de 100% na mesma semana', () => {
    const existing = [allocation({ dedicationPercent: 60 })]
    const result = findFirstWeeklyOverload('2026-06-01', '2026-06-30', 50, existing)
    expect(result).not.toBeNull()
    expect(result?.totalPercent).toBe(110)
  })

  it('ignora alocações encerradas', () => {
    const existing = [allocation({ dedicationPercent: 80, status: 'Closed' })]
    const result = findFirstWeeklyOverload('2026-06-01', '2026-06-30', 50, existing)
    expect(result).toBeNull()
  })

  it('permite exatamente 100% na semana', () => {
    const existing = [allocation({ dedicationPercent: 50 })]
    const result = findFirstWeeklyOverload('2026-06-01', '2026-06-30', 50, existing)
    expect(result).toBeNull()
  })
})

describe('peakWeeklyLoad', () => {
  it('retorna a semana com maior carga no período', () => {
    const existing = [
      allocation({
        id: 'a',
        dedicationPercent: 30,
        startDate: '2026-06-01',
        endDate: '2026-06-07',
      }),
      allocation({
        id: 'b',
        dedicationPercent: 70,
        startDate: '2026-06-15',
        endDate: '2026-06-21',
      }),
    ]

    const peak = peakWeeklyLoad('2026-06-01', '2026-06-30', existing)
    expect(peak?.allocatedPercent).toBe(70)
  })
})
