import { z } from 'zod'
import type { AllocationStatus } from '../types'

const allocationStatuses = [
  'Planned',
  'Confirmed',
  'AtRisk',
  'Closed',
] as const satisfies readonly AllocationStatus[]

export const allocationFormSchema = z
  .object({
    personId: z.string().min(1, 'Selecione uma pessoa'),
    projectId: z.string().min(1, 'Selecione um projeto'),
    role: z.string().trim().min(1, 'Papel é obrigatório'),
    dedicationPercent: z.number().min(1).max(100),
    startDate: z.string().min(1, 'Data de início obrigatória'),
    endDate: z.string().min(1, 'Data de fim obrigatória'),
    status: z.enum(allocationStatuses),
  })
  .refine((data) => data.endDate >= data.startDate, {
    message: 'Data de fim deve ser igual ou posterior ao início',
    path: ['endDate'],
  })

export type AllocationFormValues = z.infer<typeof allocationFormSchema>

export function toAllocationPayload(values: AllocationFormValues) {
  return {
    personId: values.personId,
    projectId: values.projectId,
    role: values.role.trim(),
    dedicationPercent: values.dedicationPercent,
    startDate: values.startDate,
    endDate: values.endDate,
    status: values.status,
    notes: null,
  }
}
