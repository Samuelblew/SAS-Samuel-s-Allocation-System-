import { Link } from 'react-router-dom'
import { useLocale } from '../context/LocaleContext'
import type { PageMessageKey } from '../i18n/pageMessages'
import { formatPercent } from '../lib/dates'
import type { CandidateMatch, MatchingSuggestionDecision } from '../lib/types'
import { btnGhostClass, btnSuccessClass } from '../lib/ui'
import { Card } from './Card'
import { StatusBadge } from './StatusBadge'

const scoreKeys: { key: keyof CandidateMatch['breakdown']; labelKey: PageMessageKey }[] = [
  { key: 'availabilityScore', labelKey: 'matching.score.availability' },
  { key: 'requiredSkillsScore', labelKey: 'matching.score.requiredSkills' },
  { key: 'desiredSkillsScore', labelKey: 'matching.score.desiredSkills' },
  { key: 'seniorityScore', labelKey: 'matching.score.seniority' },
  { key: 'historyScore', labelKey: 'matching.score.history' },
  { key: 'costScore', labelKey: 'matching.score.cost' },
  { key: 'overloadPenalty', labelKey: 'matching.score.overload' },
  { key: 'switchingPenalty', labelKey: 'matching.score.switching' },
]

function OnProjectBadge({ dedication }: { dedication: number | null }) {
  const { tp } = useLocale()
  return (
    <span className="ias-badge-on-project">
      {tp('matching.onProject')}
      {dedication != null ? ` · ${dedication}%` : ''}
    </span>
  )
}

function ScoreBreakdown({ breakdown }: { breakdown: CandidateMatch['breakdown'] }) {
  const { tp } = useLocale()
  return (
    <div className="mt-4 grid gap-2 sm:grid-cols-2 lg:grid-cols-4">
      {scoreKeys.map(({ key, labelKey }) => {
        const value = breakdown[key]
        const isPenalty = key.includes('Penalty')
        return (
          <div key={key} className="rounded-lg ias-list-row px-3 py-2 text-xs">
            <span className="ias-text-subtle">{tp(labelKey)}</span>
            <p
              className={`mt-0.5 font-medium ${
                isPenalty && value > 0 ? 'ias-text-warning' : 'ias-text-muted'
              }`}
            >
              {isPenalty && value > 0 ? `−${value}` : value}
            </p>
          </div>
        )
      })}
    </div>
  )
}

export interface MatchingCandidateCardProps {
  candidate: CandidateMatch
  rank: number
  decision?: MatchingSuggestionDecision
  isPending: boolean
  isPlanned: boolean
  plannedLabel: string
  onAllocate: (candidate: CandidateMatch) => void
  onReject: (candidate: CandidateMatch) => void
  dimmed?: boolean
}

export function MatchingCandidateCard({
  candidate,
  rank,
  decision,
  isPending,
  isPlanned,
  plannedLabel,
  onAllocate,
  onReject,
  dimmed,
}: MatchingCandidateCardProps) {
  const { tp } = useLocale()
  const isRejected = decision === 'Rejected'

  return (
    <Card
      className={
        isPlanned
          ? 'border-[var(--ias-border-strong)]/60 bg-[var(--ias-surface-muted)]/40'
          : isRejected
            ? 'border-[var(--ias-border)] ias-panel-inset opacity-75'
            : candidate.alreadyOnProject
              ? 'ias-border-warning'
              : rank === 1
                ? 'border-[color-mix(in_srgb,var(--ias-accent)_35%,var(--ias-border))]'
                : ''
      }
    >
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div className="min-w-0 flex-1">
          <div className="flex flex-wrap items-center gap-2">
            {rank === 1 && !isRejected && !isPlanned && (
              <span className="ias-badge-rank">#1</span>
            )}
            <h3 className={`font-semibold ${dimmed || isRejected ? 'ias-text-muted' : 'ias-text'}`}>
              {candidate.personName}
            </h3>
            {candidate.jobTitle && (
              <span className="text-sm ias-text-subtle">{candidate.jobTitle}</span>
            )}
            {candidate.seniority && (
              <span className="text-sm ias-text-subtle">{candidate.seniority}</span>
            )}
            {candidate.alreadyOnProject && !isPlanned && (
              <OnProjectBadge dedication={candidate.projectDedicationPercent} />
            )}
            {isPlanned && <StatusBadge label={plannedLabel} status="Planned" />}
            {isRejected && (
              <span className="rounded-full bg-[var(--ias-surface-hover)] px-2.5 py-0.5 text-xs font-medium ias-text-muted ring-1 ring-[var(--ias-border)]">
                {tp('matching.rejectedBadge')}
              </span>
            )}
          </div>
          <div className="mt-2 flex flex-wrap items-center gap-3 text-sm">
            <StatusBadge
              label={`${tp('matching.scoreLabel')} ${candidate.totalScore.toFixed(1)}`}
              status="Confirmed"
            />
            <span className="ias-text-muted">
              {tp('matching.minAvail')} {formatPercent(candidate.minAvailablePercent)}
            </span>
          </div>
        </div>

        <div className="flex shrink-0 flex-wrap gap-2">
          {isPlanned ? (
            <Link
              to="/allocations"
              className={`${btnGhostClass} border border-[var(--ias-border-strong)]`}
            >
              {tp('matching.viewAllocations')}
            </Link>
          ) : isRejected ? (
            <button
              type="button"
              disabled={isPending}
              onClick={() => onAllocate(candidate)}
              className={`${btnGhostClass} border border-[var(--ias-border-strong)] text-xs`}
            >
              {isPending ? tp('matching.saving') : tp('matching.reconsider')}
            </button>
          ) : (
            <>
              <button
                type="button"
                disabled={isPending}
                onClick={() => onAllocate(candidate)}
                className={btnSuccessClass}
              >
                {isPending ? tp('matching.allocating') : tp('matching.allocate')}
              </button>
              <button
                type="button"
                disabled={isPending}
                onClick={() => onReject(candidate)}
                className={btnGhostClass}
              >
                {isPending ? tp('matching.saving') : tp('matching.reject')}
              </button>
            </>
          )}
        </div>
      </div>

      {!isRejected && <ScoreBreakdown breakdown={candidate.breakdown} />}
    </Card>
  )
}
