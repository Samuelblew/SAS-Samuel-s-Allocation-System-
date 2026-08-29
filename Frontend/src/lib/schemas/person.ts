import { z } from 'zod'
import type { PersonStatus } from '../types'

const personStatuses = [
  'Active',
  'Vacation',
  'NoticePeriod',
  'Offboarded',
  'Contractor',
] as const satisfies readonly PersonStatus[]

export const personFormSchema = z.object({
  name: z.string().trim().min(1, 'Nome é obrigatório'),
  jobTitle: z.string().optional(),
  seniority: z.string().optional(),
  weeklyCapacityHours: z.number().min(1).max(168),
  status: z.enum(personStatuses),
})

export type PersonFormValues = z.infer<typeof personFormSchema>

export function toPersonPayload(values: PersonFormValues) {
  return {
    name: values.name.trim(),
    jobTitle: values.jobTitle?.trim() || null,
    seniority: values.seniority?.trim() || null,
    hourlyCost: null,
    monthlyCost: null,
    weeklyCapacityHours: values.weeklyCapacityHours,
    location: null,
    team: null,
    status: values.status,
  }
}

export const personSkillFormSchema = z.object({
  skillId: z.string().min(1, 'Selecione uma skill'),
  level: z.enum(['Beginner', 'Intermediate', 'Advanced', 'Expert']),
})

export type PersonSkillFormValues = z.infer<typeof personSkillFormSchema>
