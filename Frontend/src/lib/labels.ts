import type { Locale } from '../i18n/messages'
import { getDomainLabels } from '../i18n/domain'

/** @deprecated Use useDomainLabels() from context/LocaleContext */
export function personStatusLabels(locale: Locale = 'pt') {
  return getDomainLabels(locale).personStatus
}

/** @deprecated Use useDomainLabels() */
export const allocationNeedStatusLabels = getDomainLabels('pt').allocationNeedStatus
export const allocationNeedUrgencyLabels = getDomainLabels('pt').allocationNeedUrgency
export const allocationNeedCriticalityLabels = getDomainLabels('pt').allocationNeedCriticality
export const allocationStatusLabels = getDomainLabels('pt').allocationStatus
export const personStatusLabelsStatic = getDomainLabels('pt').personStatus
export const projectStatusLabels = getDomainLabels('pt').projectStatus
export const projectPriorityLabels = getDomainLabels('pt').projectPriority
export const skillProficiencyLabels = getDomainLabels('pt').skillProficiency
