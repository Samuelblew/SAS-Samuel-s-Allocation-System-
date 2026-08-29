export type PersonStatus =
  | 'Active'
  | 'Vacation'
  | 'NoticePeriod'
  | 'Offboarded'
  | 'Contractor'

export type ProjectStatus =
  | 'Proposal'
  | 'Approved'
  | 'InProgress'
  | 'Paused'
  | 'Closed'

export type ProjectPriority = 'Low' | 'Medium' | 'High' | 'Critical'

export type AllocationStatus = 'Planned' | 'Confirmed' | 'AtRisk' | 'Closed'

export interface Paged<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
}

export interface Skill {
  id: string
  name: string
  category: string | null
  createdAt: string
  updatedAt: string | null
}

export interface PersonListItem {
  id: string
  name: string
  jobTitle: string | null
  seniority: string | null
  weeklyCapacityHours: number
  status: PersonStatus
  skillCount: number
  createdAt: string
}

export interface Person extends PersonListItem {
  hourlyCost: number | null
  monthlyCost: number | null
  location: string | null
  skills: PersonSkill[]
  updatedAt: string | null
}

export interface PersonSkill {
  id: string
  skillId: string
  skillName: string
  skillCategory: string | null
  level: string
  lastUsedAt: string | null
  notes: string | null
}

export interface ClientListItem {
  id: string
  name: string
  projectCount: number
  createdAt: string
}

export interface ProjectListItem {
  id: string
  clientId: string
  clientName: string
  name: string
  status: ProjectStatus
  priority: ProjectPriority
  startDate: string | null
  endDate: string | null
  createdAt: string
}

export interface Project extends ProjectListItem {
  budget: number | null
  estimatedRevenue: number | null
  projectType: string | null
  commercialOwner: string | null
  deliveryOwner: string | null
  updatedAt: string | null
}

export interface AllocationListItem {
  id: string
  personId: string
  personName: string
  projectId: string
  projectName: string
  role: string
  dedicationPercent: number
  status: AllocationStatus
  startDate: string
  endDate: string
  createdAt: string
}

export interface Allocation extends AllocationListItem {
  notes: string | null
  updatedAt: string | null
}

export interface AllocationConflictItem {
  allocationId: string
  projectId: string
  projectName: string
  dedicationPercent: number
  startDate: string
  endDate: string
  status: AllocationStatus
}

export interface AllocationConflict {
  personId: string
  personName: string
  weekStart: string
  weekEnd: string
  totalDedicationPercent: number
  allocations: AllocationConflictItem[]
}

export interface ApiErrorBody {
  title?: string
  detail?: string
  status?: number
}

export type AllocationNeedStatus = 'Open' | 'PartiallyFilled' | 'Filled'
export type AllocationNeedUrgency = 'Low' | 'Medium' | 'High'
export type AllocationNeedCriticality = 'Low' | 'Medium' | 'High'

export interface AllocationNeed {
  id: string
  projectId: string
  projectName: string
  role: string
  expectedSeniority: string | null
  requiredSkillIds: string[]
  desiredSkillIds: string[]
  dedicationPercent: number
  startDate: string | null
  endDate: string | null
  urgency: AllocationNeedUrgency
  criticality: AllocationNeedCriticality
  status: AllocationNeedStatus
  createdAt: string
  updatedAt: string | null
}

export interface AllocationNeedListItem {
  id: string
  projectId: string
  projectName: string
  role: string
  expectedSeniority: string | null
  requiredSkillIds: string[]
  status: AllocationNeedStatus
  urgency: string
  criticality: string
  dedicationPercent: number
  startDate: string | null
  endDate: string | null
  createdAt: string
}

export interface PagedAllocationNeeds {
  items: AllocationNeedListItem[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
}

export interface WeekOverview {
  weekStart: string
  weekEnd: string
  activePeopleCount: number
  avgAllocatedPercent: number
  avgAvailablePercent: number
  benchPeopleCount: number
  overallocatedPeopleCount: number
  totalCapacityHours: number
  totalAllocatedHours: number
  totalAvailableHours: number
}

export interface TeamOccupation {
  team: string | null
  peopleCount: number
  avgAllocatedPercent: number
  avgAvailablePercent: number
}

export interface CapacityOverview {
  from: string
  to: string
  weeks: WeekOverview[]
  teams: TeamOccupation[]
}

export interface SkillOccupation {
  skillId: string
  skillName: string
  category: string | null
  peopleCount: number
  avgAllocatedPercent: number
  avgAvailablePercent: number
  avgAllocatedHours: number
  avgAvailableHours: number
}

export interface WeekCapacityGap {
  weekStart: string
  weekEnd: string
  totalGapDemandPercent: number
  totalAvailableSupplyPercent: number
  netShortfallPercent: number
  openNeedsInWeek: number
}

export interface OpenNeedGap {
  needId: string
  projectId: string
  projectName: string
  role: string
  requiredPercent: number
  coveredPercent: number
  gapPercent: number
  status: AllocationNeedStatus
  startDate: string | null
  endDate: string | null
}

export interface FutureCapacityGaps {
  from: string
  to: string
  peakShortfallPercent: number
  weeks: WeekCapacityGap[]
  openNeeds: OpenNeedGap[]
}

export interface SkillsOccupation {
  from: string
  to: string
  skills: SkillOccupation[]
}

export interface BenchPerson {
  personId: string
  personName: string
  team: string | null
  seniority: string | null
  minAvailablePercentInPeriod: number
  avgAvailablePercent: number
}

export interface BenchPeople {
  from: string
  to: string
  minAvailablePercent: number
  people: BenchPerson[]
}

export interface UnderstaffedProject {
  projectId: string
  projectName: string
  status: ProjectStatus
  openNeedsCount: number
  totalGapPercent: number
}

export interface CandidateScoreBreakdown {
  availabilityScore: number
  requiredSkillsScore: number
  desiredSkillsScore: number
  seniorityScore: number
  historyScore: number
  costScore: number
  overloadPenalty: number
  switchingPenalty: number
  totalScore: number
}

export interface CandidateMatch {
  personId: string
  personName: string
  jobTitle: string | null
  seniority: string | null
  minAvailablePercent: number
  alreadyOnProject: boolean
  projectDedicationPercent: number | null
  totalScore: number
  breakdown: CandidateScoreBreakdown
}

export type MatchingSuggestionDecision = 'Accepted' | 'Rejected'

export interface MatchingSuggestion {
  id: string
  allocationNeedId: string
  projectName: string
  needRole: string
  personId: string
  personName: string
  decision: MatchingSuggestionDecision
  score: number
  notes: string | null
  decidedByUserId: string | null
  createdAt: string
}

export interface PagedMatchingSuggestions {
  items: MatchingSuggestion[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
}

export interface ProjectMatchingCandidates {
  projectId: string
  projectName: string
  needs: AllocationNeedCandidates[]
}

export interface AllocationNeedCandidates {
  allocationNeedId: string
  projectId: string
  projectName: string
  role: string
  dedicationPercent: number
  periodStart: string
  periodEnd: string
  candidates: CandidateMatch[]
}

export interface ProjectFinancialSummary {
  projectId: string
  projectName: string
  clientName: string
  status: ProjectStatus
  estimatedRevenue: number | null
  totalCost: number
  marginAmount: number | null
  marginPercent: number | null
  isLowMarginAlert: boolean
}

export interface LowMarginAlert {
  projectId: string
  projectName: string
  clientName: string
  marginPercent: number | null
  totalCost: number
  estimatedRevenue: number | null
}

export interface FinancialOverview {
  periodStart: string
  periodEnd: string
  marginAlertThresholdPercent: number
  totalCost: number
  totalRevenue: number | null
  totalMargin: number | null
  avgMarginPercent: number | null
  projects: ProjectFinancialSummary[]
  lowMarginAlerts: LowMarginAlert[]
}

export interface ProfitabilityGroup {
  groupKey: string
  clientId: string | null
  projectCount: number
  totalCost: number
  totalRevenue: number | null
  totalMargin: number | null
  marginPercent: number | null
  isLowMarginAlert: boolean
}

export interface Profitability {
  periodStart: string
  periodEnd: string
  groupBy: string
  marginAlertThresholdPercent: number
  groups: ProfitabilityGroup[]
}

export interface AllocationCost {
  allocationId: string
  personId: string
  personName: string
  role: string
  dedicationPercent: number
  allocationStart: string
  allocationEnd: string
  hourlyRate: number | null
  weeksInPeriod: number
  totalHours: number
  totalCost: number
  hasCostData: boolean
}

export interface SimulatedNeedInput {
  role: string
  expectedSeniority: string | null
  requiredSkillIds: string[]
  dedicationPercent: number
  quantity: number
}

export interface RoleCandidatePreview {
  personId: string
  personName: string
  seniority: string | null
  minAvailablePercent: number
}

export interface RoleFeasibility {
  role: string
  expectedSeniority: string | null
  dedicationPercent: number
  quantityRequired: number
  candidatesAtDesiredStart: number
  satisfiedAtDesiredStart: boolean
  eligibleCandidates: RoleCandidatePreview[]
}

export interface ProjectFeasibility {
  desiredStartDate: string
  simulatedEndDate: string
  feasibleAtDesiredStart: boolean
  earliestFeasibleStart: string | null
  weeksScanned: number
  activePeopleCount: number
  benchAtDesiredStart: number
  totalHeadcountRequired: number
  roles: RoleFeasibility[]
}

export interface BenchPersonCost {
  personId: string
  personName: string
  team: string | null
  minAvailablePercent: number
  avgAvailablePercent: number
  benchHours: number
  benchCost: number
  hasCostData: boolean
}

export interface BenchCost {
  from: string
  to: string
  minAvailablePercent: number
  totalBenchHours: number
  totalBenchCost: number
  people: BenchPersonCost[]
}

export interface AllocationMarginSimulation {
  projectId: string
  projectName: string
  periodStart: string
  periodEnd: string
  currentTotalCost: number | null
  currentMarginAmount: number | null
  currentMarginPercent: number | null
  simulatedAdditionalCost: number
  projectedTotalCost: number | null
  projectedMarginAmount: number | null
  projectedMarginPercent: number | null
  marginDeltaAmount: number
  marginDeltaPercent: number | null
  hasRevenueData: boolean
  currentIsLowMarginAlert: boolean
  projectedIsLowMarginAlert: boolean
  marginAlertThresholdPercent: number
}

export interface ProjectFinancials {
  projectId: string
  projectName: string
  clientId: string
  clientName: string
  status: ProjectStatus
  periodStart: string
  periodEnd: string
  estimatedRevenue: number | null
  budget: number | null
  totalCost: number
  marginAmount: number | null
  marginPercent: number | null
  hasRevenueData: boolean
  hasCostData: boolean
  isLowMarginAlert: boolean
  marginAlertThresholdPercent: number
  allocations: AllocationCost[]
}
