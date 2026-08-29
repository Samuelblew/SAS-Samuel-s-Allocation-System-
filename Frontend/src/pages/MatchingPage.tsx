import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useCallback, useEffect, useMemo, useState } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import { Alert } from '../components/Alert'
import { Card } from '../components/Card'
import { LoadingState } from '../components/LoadingState'
import { PageHeader } from '../components/PageHeader'
import { PageGuide } from '../components/page/PageGuide'
import { PageHero, PageHeroMetric } from '../components/page/PageHero'
import { PageSection } from '../components/page/PageSection'
import { useDomainLabels, useLocale } from '../context/LocaleContext'
import { useApiOptions, useSettings } from '../context/SettingsContext'
import { LIST_PAGE_SIZE } from '../lib/constants'
import { MatchingCandidateCard } from '../components/MatchingCandidateCard'
import { api, ApiError } from '../lib/api'
import { getErrorMessage } from '../lib/errors'
import { formatPercent } from '../lib/dates'
import type { PageMessageKey } from '../i18n/pageMessages'
import { inputClass } from '../lib/ui'
import type {
  AllocationListItem,
  AllocationNeedCandidates,
  CandidateMatch,
  MatchingSuggestionDecision,
  Paged,
  PagedAllocationNeeds,
  PagedMatchingSuggestions,
  ProjectListItem,
  ProjectMatchingCandidates,
} from '../lib/types'

function formatActionError(
  err: unknown,
  tp: (key: PageMessageKey, vars?: Record<string, string | number>) => string,
): string {
  if (err instanceof ApiError) {
    const msg = err.message
    if (msg.includes('RN-001') || msg.includes('ultrapassa')) {
      return tp('matching.error.overload')
    }
    if (msg.includes('inativa')) {
      return tp('matching.error.inactive')
    }
    return msg.length > 200 ? `${msg.slice(0, 200)}…` : msg
  }
  return getErrorMessage(err)
}

function latestDecisionsByPerson(
  items: PagedMatchingSuggestions['items'],
): Map<string, MatchingSuggestionDecision> {
  const map = new Map<string, MatchingSuggestionDecision>()
  for (const item of items) {
    if (!map.has(item.personId)) {
      map.set(item.personId, item.decision)
    }
  }
  return map
}

export function MatchingPage() {
  const { t, tp } = useLocale()
  const {
    allocationNeedStatus: allocationNeedStatusLabels,
    allocationStatus: allocationStatusLabels,
  } = useDomainLabels()
  const { isTenantValid } = useSettings()
  const opts = useApiOptions()
  const qc = useQueryClient()
  const [searchParams, setSearchParams] = useSearchParams()

  const [selectedNeedId, setSelectedNeedId] = useState(searchParams.get('needId') ?? '')
  const [minAvailable, setMinAvailable] = useState('')
  const [excludeOnProject, setExcludeOnProject] = useState(false)
  const [showRejected, setShowRejected] = useState(false)
  const [showProjectOverview, setShowProjectOverview] = useState(false)
  const [selectedProjectId, setSelectedProjectId] = useState('')
  const [feedback, setFeedback] = useState<{ type: 'error' | 'success'; text: string } | null>(null)
  const [pendingPersonId, setPendingPersonId] = useState<string | null>(null)
  const [localDecisions, setLocalDecisions] = useState<Map<string, MatchingSuggestionDecision>>(
    new Map(),
  )

  useEffect(() => {
    const fromUrl = searchParams.get('needId') ?? ''
    if (fromUrl && fromUrl !== selectedNeedId) {
      setSelectedNeedId(fromUrl)
    }
  }, [searchParams, selectedNeedId])

  const selectNeed = useCallback(
    (needId: string) => {
      setSelectedNeedId(needId)
      setLocalDecisions(new Map())
      setFeedback(null)
      setPendingPersonId(null)
      if (needId) {
        setSearchParams({ needId }, { replace: true })
      } else {
        setSearchParams({}, { replace: true })
      }
    },
    [setSearchParams],
  )

  const projects = useQuery({
    queryKey: ['projects', opts.tenantId],
    enabled: isTenantValid,
    queryFn: () =>
      api.get<Paged<ProjectListItem>>(`/api/v1/projects?page=1&pageSize=${LIST_PAGE_SIZE}`, opts),
  })

  const needs = useQuery({
    queryKey: ['allocation-needs', opts.tenantId],
    enabled: isTenantValid,
    queryFn: () =>
      api.get<PagedAllocationNeeds>('/api/v1/allocation-needs?page=1&pageSize=100', opts),
  })

  const openNeeds = useMemo(
    () => needs.data?.items.filter((n) => n.status !== 'Filled') ?? [],
    [needs.data?.items],
  )
  const selectedNeed = openNeeds.find((n) => n.id === selectedNeedId)

  const candidatesQueryKey = [
    'matching-candidates',
    opts.tenantId,
    selectedNeedId,
    minAvailable,
    excludeOnProject,
  ] as const

  const candidates = useQuery({
    queryKey: candidatesQueryKey,
    enabled: isTenantValid && !!selectedNeedId,
    queryFn: () => {
      const params = new URLSearchParams({ maxResults: '20' })
      if (minAvailable.trim()) {
        params.set('minAvailablePercent', minAvailable.trim())
      }
      if (excludeOnProject) {
        params.set('excludePeopleOnProject', 'true')
      }
      return api.get<AllocationNeedCandidates>(
        `/api/v1/allocation-needs/${selectedNeedId}/candidates?${params.toString()}`,
        opts,
      )
    },
  })

  const history = useQuery({
    queryKey: ['matching-suggestions', opts.tenantId, selectedNeedId],
    enabled: isTenantValid && !!selectedNeedId,
    queryFn: () =>
      api.get<PagedMatchingSuggestions>(
        `/api/v1/allocation-needs/${selectedNeedId}/matching-suggestions?page=1&pageSize=100`,
        opts,
      ),
  })

  const projectAllocations = useQuery({
    queryKey: ['allocations', opts.tenantId, selectedNeed?.projectId],
    enabled: isTenantValid && !!selectedNeed?.projectId,
    queryFn: () =>
      api.get<Paged<AllocationListItem>>('/api/v1/allocations?page=1&pageSize=100', opts),
  })

  const plannedPersonIds = useMemo(() => {
    const ids = new Set<string>()
    if (!projectAllocations.data || !selectedNeed) return ids
    for (const a of projectAllocations.data.items) {
      if (
        a.projectId === selectedNeed.projectId &&
        a.role === selectedNeed.role &&
        a.status === 'Planned'
      ) {
        ids.add(a.personId)
      }
    }
    return ids
  }, [projectAllocations.data, selectedNeed])

  useEffect(() => {
    if (!history.data) return
    setLocalDecisions(latestDecisionsByPerson(history.data.items))
  }, [history.data])

  const decisions = localDecisions

  const recordDecision = useMutation({
    mutationFn: ({
      personId,
      decision,
      score,
    }: {
      personId: string
      decision: MatchingSuggestionDecision
      score: number
    }) =>
      api.post(
        `/api/v1/allocation-needs/${selectedNeedId}/matching-suggestions`,
        { personId, decision, score },
        opts,
      ),
    onMutate: ({ personId, decision }) => {
      setPendingPersonId(personId)
      setFeedback(null)
      setLocalDecisions((prev) => new Map(prev).set(personId, decision))
    },
    onSuccess: async () => {
      await qc.invalidateQueries({ queryKey: ['matching-suggestions', opts.tenantId, selectedNeedId] })
    },
    onError: (err: Error, { personId }) => {
      setLocalDecisions((prev) => {
        const next = new Map(prev)
        next.delete(personId)
        return next
      })
      setFeedback({
        type: 'error',
        text: formatActionError(err, tp),
      })
    },
    onSettled: () => setPendingPersonId(null),
  })

  const invalidateAfterAllocate = useCallback(async () => {
    await Promise.all([
      qc.invalidateQueries({ queryKey: ['matching-suggestions', opts.tenantId, selectedNeedId] }),
      qc.invalidateQueries({ queryKey: ['allocations'] }),
      qc.invalidateQueries({ queryKey: ['allocation-needs'] }),
      qc.invalidateQueries({ queryKey: ['matching-candidates'] }),
      qc.invalidateQueries({ queryKey: ['dashboard-counts'] }),
      qc.invalidateQueries({ queryKey: ['conflicts'] }),
      qc.invalidateQueries({ queryKey: ['capacity-understaffed'] }),
    ])
  }, [qc, opts.tenantId, selectedNeedId])

  const handleAllocate = useCallback(
    async (candidate: CandidateMatch) => {
      if (!candidates.data || !selectedNeedId) return
      if (plannedPersonIds.has(candidate.personId)) return

      const data = candidates.data

      try {
        setPendingPersonId(candidate.personId)
        setFeedback(null)
        setLocalDecisions((prev) => new Map(prev).set(candidate.personId, 'Accepted'))

        await api.post(
          '/api/v1/allocations',
          {
            personId: candidate.personId,
            projectId: data.projectId,
            role: data.role,
            dedicationPercent: data.dedicationPercent,
            startDate: data.periodStart,
            endDate: data.periodEnd,
            status: 'Planned',
            notes: null,
          },
          opts,
        )

        try {
          await api.post(
            `/api/v1/allocation-needs/${selectedNeedId}/matching-suggestions`,
            {
              personId: candidate.personId,
              decision: 'Accepted',
              score: candidate.totalScore,
            },
            opts,
          )
        } catch {
          // Alocação criada; histórico de matching é secundário
        }

        await invalidateAfterAllocate()

        setFeedback({
          type: 'success',
          text: tp('matching.feedback.allocated', { name: candidate.personName }),
        })
      } catch (err) {
        setLocalDecisions((prev) => {
          const next = new Map(prev)
          next.delete(candidate.personId)
          return next
        })
        setFeedback({
          type: 'error',
          text: formatActionError(err, tp),
        })
      } finally {
        setPendingPersonId(null)
      }
    },
    [candidates.data, selectedNeedId, opts, plannedPersonIds, invalidateAfterAllocate, tp],
  )

  const handleReject = useCallback(
    (candidate: CandidateMatch) => {
      recordDecision.mutate({
        personId: candidate.personId,
        decision: 'Rejected',
        score: candidate.totalScore,
      })
      setFeedback({
        type: 'success',
        text: tp('matching.feedback.rejected', { name: candidate.personName }),
      })
    },
    [recordDecision, tp],
  )

  const batchCandidates = useQuery({
    queryKey: ['project-matching-candidates', opts.tenantId, selectedProjectId],
    enabled: isTenantValid && showProjectOverview && !!selectedProjectId,
    queryFn: () =>
      api.get<ProjectMatchingCandidates>(
        `/api/v1/projects/${selectedProjectId}/matching-candidates?maxResultsPerNeed=5`,
        opts,
      ),
  })

  const { activeCandidates, plannedCandidates, rejectedCandidates } = useMemo(() => {
    const list = candidates.data?.candidates ?? []
    const active: CandidateMatch[] = []
    const planned: CandidateMatch[] = []
    const rejected: CandidateMatch[] = []

    for (const c of list) {
      if (plannedPersonIds.has(c.personId)) {
        planned.push(c)
      } else if (decisions.get(c.personId) === 'Rejected') {
        rejected.push(c)
      } else {
        active.push(c)
      }
    }

    return { activeCandidates: active, plannedCandidates: planned, rejectedCandidates: rejected }
  }, [candidates.data, decisions, plannedPersonIds])

  const projectsWithOpenNeeds = useMemo(() => {
    const ids = new Set(openNeeds.map((n) => n.projectId))
    return projects.data?.items.filter((p) => ids.has(p.id)) ?? []
  }, [openNeeds, projects.data])

  return (
    <div>
      <PageHeader
        title={t('pages.matching.title')}
        description={t('pages.matching.description')}
        hideTitle
      />

      <PageGuide
        title={t('guide.howToRead')}
        steps={[tp('matching.guide.step1'), tp('matching.guide.step2'), tp('matching.guide.step3')]}
      />

      {needs.isSuccess && (
        <PageHero
          label={tp('matching.hero.label')}
          value={openNeeds.length}
          hint={tp('matching.hero.hint')}
          tone={openNeeds.length > 0 ? 'balanced' : 'success'}
          metrics={
            <PageHeroMetric
              label={tp('matching.hero.openNeeds')}
              value={openNeeds.length}
              valueClassName={openNeeds.length > 0 ? 'ias-text-warning' : ''}
            />
          }
        />
      )}

      {needs.isLoading && <LoadingState />}
      {needs.isError && <Alert message={(needs.error as Error).message} />}
      {feedback?.type === 'error' && (
        <div className="mb-4">
          <Alert message={feedback.text} />
        </div>
      )}
      {feedback?.type === 'success' && (
        <div className="mb-4">
          <Alert message={feedback.text} variant="success" />
        </div>
      )}

      {needs.isSuccess && (
        <PageSection title={tp('matching.selectNeed')} className="mb-6">
          <div className="space-y-4">
          <div className="grid gap-4 lg:grid-cols-2">
            <label className="flex flex-col gap-2 text-sm">
              <span className="ias-text-muted">{tp('matching.selectNeed')}</span>
              <select
                value={selectedNeedId}
                onChange={(e) => selectNeed(e.target.value)}
                className={inputClass}
              >
                <option value="">{tp('matching.selectOpenNeed')}</option>
                {openNeeds.map((n) => (
                  <option key={n.id} value={n.id}>
                    {n.projectName} · {n.role} · {allocationNeedStatusLabels[n.status] ?? n.status}
                  </option>
                ))}
              </select>
            </label>

            <div className="flex flex-wrap items-end gap-3">
              <label className="flex flex-col gap-2 text-sm">
                <span className="ias-text-muted">{tp('matching.minAvailability')}</span>
                <input
                  type="number"
                  min={0}
                  max={100}
                  placeholder={tp('matching.minAvailabilityPlaceholder')}
                  value={minAvailable}
                  onChange={(e) => setMinAvailable(e.target.value)}
                  className={`w-full sm:w-28 ${inputClass}`}
                  disabled={!selectedNeedId}
                />
              </label>
              <label className="flex items-center gap-2 pb-2 text-sm ias-text-muted">
                <input
                  type="checkbox"
                  checked={excludeOnProject}
                  onChange={(e) => setExcludeOnProject(e.target.checked)}
                  disabled={!selectedNeedId}
                  className="rounded border-[var(--ias-border-strong)]"
                />
                {tp('matching.excludeOnProject')}
              </label>
            </div>
          </div>

          {openNeeds.length === 0 && (
            <p className="text-sm ias-text-subtle">
              {tp('matching.noOpenNeeds')}{' '}
              <Link to="/allocation-needs" className="ias-link">
                {t('nav.needs')}
              </Link>
              .
            </p>
          )}

          {selectedNeed && (
            <div className="grid gap-2 rounded-lg border border-[var(--ias-border)] ias-panel p-3 p-3 text-sm sm:grid-cols-2 lg:grid-cols-5">
              <div>
                <p className="text-xs ias-text-subtle">{t('common.project')}</p>
                <p className="ias-text">{selectedNeed.projectName}</p>
              </div>
              <div>
                <p className="text-xs ias-text-subtle">{t('common.role')}</p>
                <p className="ias-text">{selectedNeed.role}</p>
              </div>
              <div>
                <p className="text-xs ias-text-subtle">{tp('matching.requiredDedication')}</p>
                <p className="ias-text-muted">{selectedNeed.dedicationPercent}%</p>
              </div>
              <div>
                <p className="text-xs ias-text-subtle">{t('common.period')}</p>
                <p className="ias-text-muted">
                  {selectedNeed.startDate ?? '—'} → {selectedNeed.endDate ?? '—'}
                </p>
              </div>
              <div>
                <p className="text-xs ias-text-subtle">{t('common.seniority')}</p>
                <p className="ias-text-muted">{selectedNeed.expectedSeniority ?? '—'}</p>
              </div>
            </div>
          )}
          </div>
        </PageSection>
      )}

      {!selectedNeedId && needs.isSuccess && openNeeds.length > 0 && (
        <Card>
          <p className="text-sm ias-text-muted">{tp('matching.selectNeedPrompt')}</p>
        </Card>
      )}

      {selectedNeedId && candidates.isLoading && (
        <LoadingState />
      )}
      {selectedNeedId && candidates.isError && (
        <Alert message={(candidates.error as Error).message} />
      )}

      {selectedNeedId && candidates.isSuccess && (
        <div className="space-y-6">
          {plannedCandidates.length > 0 && (
            <section>
              <h2 className="mb-3 text-sm font-semibold ias-text-muted">
                {tp('matching.plannedAllocations', { count: plannedCandidates.length })}
              </h2>
              <div className="space-y-4">
                {plannedCandidates.map((c) => (
                  <MatchingCandidateCard
                    key={c.personId}
                    candidate={c}
                    rank={0}
                    isPending={pendingPersonId === c.personId}
                    isPlanned
                    plannedLabel={allocationStatusLabels.Planned}
                    onAllocate={handleAllocate}
                    onReject={handleReject}
                  />
                ))}
              </div>
            </section>
          )}

          {activeCandidates.length === 0 &&
          plannedCandidates.length === 0 &&
          rejectedCandidates.length === 0 ? (
            <Card>
              <p className="text-sm ias-text-subtle">{tp('matching.noCandidates')}</p>
            </Card>
          ) : (
            activeCandidates.length > 0 && (
              <section>
                <h2 className="mb-3 text-sm font-semibold ias-text-muted">
                  {tp('matching.rankedCandidates', { count: activeCandidates.length })}
                </h2>
                <div className="space-y-4">
                  {activeCandidates.map((c, index) => (
                    <MatchingCandidateCard
                      key={c.personId}
                      candidate={c}
                      rank={index + 1}
                      isPending={pendingPersonId === c.personId}
                      isPlanned={false}
                      plannedLabel={allocationStatusLabels.Planned}
                      onAllocate={handleAllocate}
                      onReject={handleReject}
                    />
                  ))}
                </div>
              </section>
            )
          )}

          {rejectedCandidates.length > 0 && (
            <section>
              <button
                type="button"
                onClick={() => setShowRejected((v) => !v)}
                className="mb-3 text-sm ias-text-muted hover:ias-text"
              >
                {showRejected ? '▾' : '▸'} {tp('matching.rejected', { count: rejectedCandidates.length })}
              </button>
              {showRejected && (
                <div className="space-y-4">
                  {rejectedCandidates.map((c) => (
                    <MatchingCandidateCard
                      key={c.personId}
                      candidate={c}
                      rank={0}
                      decision="Rejected"
                      isPending={pendingPersonId === c.personId}
                      isPlanned={false}
                      plannedLabel={allocationStatusLabels.Planned}
                      onAllocate={handleAllocate}
                      onReject={handleReject}
                      dimmed
                    />
                  ))}
                </div>
              )}
            </section>
          )}
        </div>
      )}

      {selectedNeedId && history.isSuccess && history.data.items.length > 0 && (
        <Card className="mt-8">
          <h3 className="mb-4 text-sm font-semibold ias-text-muted">{tp('matching.history')}</h3>
          <ul className="space-y-2">
            {history.data.items.map((h) => (
              <li
                key={h.id}
                className="flex flex-wrap items-center justify-between gap-2 rounded-lg ias-list-row px-3 py-2 text-sm"
              >
                <span className="ias-text">{h.personName}</span>
                <span className="flex items-center gap-3 ias-text-muted">
                  <span className={h.decision === 'Accepted' ? 'ias-text-success' : 'ias-text-subtle'}>
                    {h.decision === 'Accepted'
                      ? tp('matching.decisionAccepted')
                      : tp('matching.decisionRejected')}
                  </span>
                  <span>score {h.score.toFixed(1)}</span>
                  <span className="text-xs ias-text-subtle">
                    {new Date(h.createdAt).toLocaleString('pt-BR')}
                  </span>
                </span>
              </li>
            ))}
          </ul>
        </Card>
      )}

      <Card className="mt-8">
        <button
          type="button"
          onClick={() => setShowProjectOverview((v) => !v)}
          className="flex w-full items-center justify-between text-left text-sm font-semibold ias-text-muted"
        >
          <span>{tp('matching.projectOverview')}</span>
          <span className="ias-text-subtle">{showProjectOverview ? '▾' : '▸'}</span>
        </button>
        {showProjectOverview && (
          <div className="mt-4 space-y-4">
            <p className="text-xs ias-text-subtle">{tp('matching.projectOverviewHint')}</p>
            <label className="flex flex-col gap-2 text-sm">
              <span className="ias-text-muted">{t('common.project')}</span>
              <select
                value={selectedProjectId}
                onChange={(e) => setSelectedProjectId(e.target.value)}
                className={inputClass}
              >
                <option value="">{t('common.select')}</option>
                {projectsWithOpenNeeds.map((p) => (
                  <option key={p.id} value={p.id}>
                    {p.name}
                  </option>
                ))}
              </select>
            </label>
            {batchCandidates.isLoading && selectedProjectId && (
              <p className="text-sm ias-text-subtle">{tp('matching.calculating')}</p>
            )}
            {batchCandidates.isError && <Alert message={(batchCandidates.error as Error).message} />}
            {batchCandidates.isSuccess && selectedProjectId && (
              <div className="space-y-4">
                {batchCandidates.data.needs.length === 0 ? (
                  <p className="text-sm ias-text-subtle">{t('empty.needs')}</p>
                ) : (
                  batchCandidates.data.needs.map((needBlock) => (
                    <div
                      key={needBlock.allocationNeedId}
                      className="rounded-lg border border-[var(--ias-border)] ias-panel p-3 p-3"
                    >
                      <div className="mb-2 flex flex-wrap items-center justify-between gap-2">
                        <h4 className="font-medium ias-text">
                          {needBlock.role}{' '}
                          <span className="text-sm font-normal ias-text-subtle">
                            {needBlock.periodStart} → {needBlock.periodEnd}
                          </span>
                        </h4>
                        <button
                          type="button"
                          onClick={() => selectNeed(needBlock.allocationNeedId)}
                          className="text-xs ias-link"
                        >
                          {tp('matching.openInMatching')}
                        </button>
                      </div>
                      {needBlock.candidates.length === 0 ? (
                        <p className="text-sm ias-text-subtle">{tp('matching.noCandidatesBatch')}</p>
                      ) : (
                        <ul className="space-y-1">
                          {needBlock.candidates.map((c, i) => (
                            <li
                              key={c.personId}
                              className="flex flex-wrap items-center justify-between gap-2 rounded ias-list-row px-2 py-1.5 text-sm"
                            >
                              <span className="ias-text-muted">
                                {i === 0 && <span className="mr-1 ias-link">#1</span>}
                                {c.personName}
                                {c.seniority && (
                                  <span className="ml-1 ias-text-subtle">{c.seniority}</span>
                                )}
                              </span>
                              <span className="ias-text-subtle">
                                {c.totalScore.toFixed(1)} · disp. {formatPercent(c.minAvailablePercent)}
                              </span>
                            </li>
                          ))}
                        </ul>
                      )}
                    </div>
                  ))
                )}
              </div>
            )}
          </div>
        )}
      </Card>
    </div>
  )
}
