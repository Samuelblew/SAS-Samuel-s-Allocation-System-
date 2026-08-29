import { describe, expect, it } from 'vitest'
import { personFormSchema, toPersonPayload } from './person'

describe('personFormSchema', () => {
  it('aceita pessoa válida', () => {
    const result = personFormSchema.safeParse({
      name: 'Ana',
      jobTitle: 'Dev',
      seniority: 'Senior',
      weeklyCapacityHours: 40,
      status: 'Active',
    })
    expect(result.success).toBe(true)
  })

  it('rejeita horas semanais inválidas', () => {
    const result = personFormSchema.safeParse({
      name: 'Ana',
      weeklyCapacityHours: 0,
      status: 'Active',
    })
    expect(result.success).toBe(false)
  })
})

describe('toPersonPayload', () => {
  it('converte campos opcionais para null', () => {
    expect(
      toPersonPayload({
        name: 'Ana',
        jobTitle: '',
        seniority: '',
        weeklyCapacityHours: 32,
        status: 'Active',
      }),
    ).toMatchObject({
      name: 'Ana',
      jobTitle: null,
      seniority: null,
      weeklyCapacityHours: 32,
      status: 'Active',
    })
  })
})
