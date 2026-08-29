import { z } from 'zod'

export const skillFormSchema = z.object({
  name: z.string().trim().min(1, 'Nome é obrigatório'),
  category: z.string().optional(),
})

export type SkillFormValues = z.infer<typeof skillFormSchema>

export function toSkillPayload(values: SkillFormValues) {
  return {
    name: values.name.trim(),
    category: values.category?.trim() || null,
  }
}
