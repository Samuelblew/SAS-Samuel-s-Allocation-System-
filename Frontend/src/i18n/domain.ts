import type {
  AllocationStatus,
  PersonStatus,
  ProjectPriority,
  ProjectStatus,
} from '../lib/types'
import type { Locale } from './messages'

const personStatus = {
  pt: {
    Active: 'Ativo',
    Vacation: 'Férias',
    NoticePeriod: 'Aviso prévio',
    Offboarded: 'Desligado',
    Contractor: 'Contratado',
  } satisfies Record<PersonStatus, string>,
  en: {
    Active: 'Active',
    Vacation: 'Vacation',
    NoticePeriod: 'Notice period',
    Offboarded: 'Offboarded',
    Contractor: 'Contractor',
  } satisfies Record<PersonStatus, string>,
}

const projectStatus = {
  pt: {
    Proposal: 'Proposta',
    Approved: 'Aprovado',
    InProgress: 'Em andamento',
    Paused: 'Pausado',
    Closed: 'Encerrado',
  } satisfies Record<ProjectStatus, string>,
  en: {
    Proposal: 'Proposal',
    Approved: 'Approved',
    InProgress: 'In progress',
    Paused: 'Paused',
    Closed: 'Closed',
  } satisfies Record<ProjectStatus, string>,
}

const projectPriority = {
  pt: {
    Low: 'Baixa',
    Medium: 'Média',
    High: 'Alta',
    Critical: 'Crítica',
  } satisfies Record<ProjectPriority, string>,
  en: {
    Low: 'Low',
    Medium: 'Medium',
    High: 'High',
    Critical: 'Critical',
  } satisfies Record<ProjectPriority, string>,
}

const allocationStatus = {
  pt: {
    Planned: 'Planejada',
    Confirmed: 'Confirmada',
    AtRisk: 'Em risco',
    Closed: 'Encerrada',
  } satisfies Record<AllocationStatus, string>,
  en: {
    Planned: 'Planned',
    Confirmed: 'Confirmed',
    AtRisk: 'At risk',
    Closed: 'Closed',
  } satisfies Record<AllocationStatus, string>,
}

const allocationNeedStatus: Record<Locale, Record<string, string>> = {
  pt: { Open: 'Aberta', PartiallyFilled: 'Parcial', Filled: 'Preenchida' },
  en: { Open: 'Open', PartiallyFilled: 'Partial', Filled: 'Filled' },
}

const allocationNeedUrgency: Record<Locale, Record<string, string>> = {
  pt: { Low: 'Baixa', Medium: 'Média', High: 'Alta' },
  en: { Low: 'Low', Medium: 'Medium', High: 'High' },
}

const allocationNeedCriticality: Record<Locale, Record<string, string>> = {
  pt: { Low: 'Baixa', Medium: 'Média', High: 'Alta' },
  en: { Low: 'Low', Medium: 'Medium', High: 'High' },
}

const skillProficiency: Record<Locale, Record<string, string>> = {
  pt: {
    Beginner: 'Iniciante',
    Intermediate: 'Intermediário',
    Advanced: 'Avançado',
    Expert: 'Especialista',
  },
  en: {
    Beginner: 'Beginner',
    Intermediate: 'Intermediate',
    Advanced: 'Advanced',
    Expert: 'Expert',
  },
}

export function getDomainLabels(locale: Locale) {
  return {
    personStatus: personStatus[locale],
    projectStatus: projectStatus[locale],
    projectPriority: projectPriority[locale],
    allocationStatus: allocationStatus[locale],
    allocationNeedStatus: allocationNeedStatus[locale],
    allocationNeedUrgency: allocationNeedUrgency[locale],
    allocationNeedCriticality: allocationNeedCriticality[locale],
    skillProficiency: skillProficiency[locale],
  }
}
