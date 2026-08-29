import { describe, expect, it } from 'vitest'
import { allocationFormSchema, toAllocationPayload } from './allocation'

describe('allocationFormSchema', () => {
  const valid = {
    personId: 'a',
    projectId: 'b',
    role: 'Backend',
    dedicationPercent: 50,
    startDate: '2026-06-01',
    endDate: '2026-06-30',
    status: 'Planned' as const,
  }

  it('aceita alocação válida', () => {
    expect(allocationFormSchema.safeParse(valid).success).toBe(true)
  })

  it('rejeita fim antes do início', () => {
    const result = allocationFormSchema.safeParse({
      ...valid,
      startDate: '2026-07-01',
      endDate: '2026-06-01',
    })
    expect(result.success).toBe(false)
  })
})

describe('toAllocationPayload', () => {
  it('normaliza role e inclui notes null', () => {
    expect(
      toAllocationPayload({
        personId: 'p1',
        projectId: 'pr1',
        role: '  Dev  ',
        dedicationPercent: 80,
        startDate: '2026-01-01',
        endDate: '2026-03-01',
        status: 'Confirmed',
      }),
    ).toMatchObject({ role: 'Dev', notes: null, status: 'Confirmed' })
  })
})
