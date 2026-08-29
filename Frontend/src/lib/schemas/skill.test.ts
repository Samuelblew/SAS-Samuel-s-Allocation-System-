import { describe, expect, it } from 'vitest'
import { skillFormSchema, toSkillPayload } from './skill'

describe('skillFormSchema', () => {
  it('aceita nome válido', () => {
    const result = skillFormSchema.safeParse({ name: 'React', category: 'Frontend' })
    expect(result.success).toBe(true)
  })

  it('rejeita nome vazio', () => {
    const result = skillFormSchema.safeParse({ name: '   ', category: '' })
    expect(result.success).toBe(false)
  })
})

describe('toSkillPayload', () => {
  it('normaliza categoria vazia para null', () => {
    expect(toSkillPayload({ name: 'Go', category: '  ' })).toEqual({
      name: 'Go',
      category: null,
    })
  })
})
