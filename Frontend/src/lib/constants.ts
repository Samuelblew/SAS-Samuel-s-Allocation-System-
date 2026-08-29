export const LIST_PAGE_SIZE = 100

export const SENIORITY_OPTIONS = ['Estagiário', 'Junior', 'Pleno', 'Senior'] as const

export type SeniorityOption = (typeof SENIORITY_OPTIONS)[number]

export const SKILL_PROFICIENCY_LEVELS = [
  'Beginner',
  'Intermediate',
  'Advanced',
  'Expert',
] as const

export type SkillProficiencyLevel = (typeof SKILL_PROFICIENCY_LEVELS)[number]
