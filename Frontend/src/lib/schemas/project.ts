import { z } from 'zod'
import type { ProjectPriority, ProjectStatus } from '../types'

const projectStatuses = [
  'Proposal',
  'Approved',
  'InProgress',
  'Paused',
  'Closed',
] as const satisfies readonly ProjectStatus[]

const projectPriorities = ['Low', 'Medium', 'High', 'Critical'] as const satisfies readonly ProjectPriority[]

export const clientFormSchema = z.object({
  name: z.string().trim().min(1, 'Nome é obrigatório'),
})

export type ClientFormValues = z.infer<typeof clientFormSchema>

export const projectFormSchema = z.object({
  clientId: z.string().min(1, 'Selecione um cliente'),
  name: z.string().trim().min(1, 'Nome é obrigatório'),
  status: z.enum(projectStatuses),
  priority: z.enum(projectPriorities),
  startDate: z.string().optional(),
  endDate: z.string().optional(),
})

export type ProjectFormValues = z.infer<typeof projectFormSchema>

export function toProjectPayload(values: ProjectFormValues) {
  return {
    clientId: values.clientId,
    name: values.name.trim(),
    status: values.status,
    startDate: values.startDate || null,
    endDate: values.endDate || null,
    priority: values.priority,
    budget: null,
    estimatedRevenue: null,
    projectType: null,
    commercialOwner: null,
    deliveryOwner: null,
  }
}
