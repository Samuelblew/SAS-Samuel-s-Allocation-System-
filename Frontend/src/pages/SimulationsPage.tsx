import { useMutation, useQuery } from '@tanstack/react-query'
import { useMemo, useState, type FormEvent } from 'react'
import { Alert } from '../components/Alert'
import { Card } from '../components/Card'
import { PageHeader } from '../components/PageHeader'
import { PageGuide } from '../components/page/PageGuide'
import { PageHero, PageHeroMetric } from '../components/page/PageHero'
import { PageSection } from '../components/page/PageSection'
import { useLocale } from '../context/LocaleContext'
import { useApiOptions, useSettings } from '../context/SettingsContext'
import { LIST_PAGE_SIZE, SENIORITY_OPTIONS } from '../lib/constants'
import { api } from '../lib/api'
import { getErrorMessage } from '../lib/errors'
import { formatPercent } from '../lib/dates'
import { btnPrimaryClass, chipClass, formRowClass, inputClass, labelClass } from '../lib/ui'
import type { Paged, ProjectFeasibility, RoleFeasibility, Skill } from '../lib/types'

interface RoleRow {
  id: string
  role: string
  seniority: string
  dedication: string
  quantity: string
  requiredSkillIds: string[]
}

function newRoleRow(): RoleRow {
  return {
    id: crypto.randomUUID(),
    role: 'Backend',
    seniority: 'Senior',
    dedication: '50',
    quantity: '1',
    requiredSkillIds: [],
  }
}

function todayIso(): string {
  return new Date().toISOString().slice(0, 10)
}

function shortfall(role: RoleFeasibility): number {
  return Math.max(0, role.quantityRequired - role.candidatesAtDesiredStart)
}

function RoleProgressBar({ role }: { role: RoleFeasibility }) {
  const { tp } = useLocale()
  const pct = Math.min(100, (role.candidatesAtDesiredStart / role.quantityRequired) * 100)
  const ok = role.satisfiedAtDesiredStart

  return (
    <div className="mt-3">
      <div className="mb-1 flex justify-between text-xs ias-text-subtle">
        <span>
          {role.candidatesAtDesiredStart}/{role.quantityRequired}{' '}
          {tp('sim.eligibleCandidates').toLowerCase()}
        </span>
        <span className={ok ? 'ias-text-success' : 'ias-text-warning'}>
          {ok ? tp('sim.covered') : tp('sim.missing', { count: shortfall(role) })}
        </span>
      </div>
      <div className="ias-progress-track">
        <div
          className={`${ok ? 'ias-progress-fill-success' : 'ias-progress-fill-warning'} transition-all`}
          style={{ width: `${pct}%` }}
        />
      </div>
    </div>
  )
}

export function SimulationsPage() {
  const { t, tp } = useLocale()
  const { isTenantValid } = useSettings()
  const opts = useApiOptions()

  const [desiredStart, setDesiredStart] = useState(todayIso())
  const [durationMonths, setDurationMonths] = useState('3')
  const [roles, setRoles] = useState<RoleRow[]>([newRoleRow()])
  const [formError, setFormError] = useState<string | null>(null)
  const [result, setResult] = useState<ProjectFeasibility | null>(null)

  const skills = useQuery({
    queryKey: ['skills', opts.tenantId],
    enabled: isTenantValid,
    queryFn: () => api.get<Paged<Skill>>(`/api/v1/skills?page=1&pageSize=${LIST_PAGE_SIZE}`, opts),
  })

  const simulate = useMutation({
    mutationFn: (body: Record<string, unknown>) =>
      api.post<ProjectFeasibility>('/api/v1/simulations/project-feasibility', body, opts),
    onSuccess: (data) => {
      setFormError(null)
      setResult(data)
    },
    onError: (err: Error) => {
      setFormError(getErrorMessage(err))
      setResult(null)
    },
  })

  const totalGap = useMemo(
    () => result?.roles.reduce((sum, r) => sum + shortfall(r), 0) ?? 0,
    [result],
  )

  function updateRole(id: string, patch: Partial<RoleRow>) {
    setRoles((prev) => prev.map((r) => (r.id === id ? { ...r, ...patch } : r)))
  }

  function toggleRoleSkill(roleId: string, skillId: string) {
    setRoles((prev) =>
      prev.map((r) => {
        if (r.id !== roleId) return r
        const has = r.requiredSkillIds.includes(skillId)
        return {
          ...r,
          requiredSkillIds: has
            ? r.requiredSkillIds.filter((id) => id !== skillId)
            : [...r.requiredSkillIds, skillId],
        }
      }),
    )
  }

  function onSubmit(e: FormEvent) {
    e.preventDefault()
    simulate.mutate({
      desiredStartDate: desiredStart,
      durationMonths: Number(durationMonths),
      needs: roles.map((r) => ({
        role: r.role.trim(),
        expectedSeniority: r.seniority || null,
        requiredSkillIds: r.requiredSkillIds,
        dedicationPercent: Number(r.dedication),
        quantity: Number(r.quantity),
      })),
    })
  }

  return (
    <div>
      <PageHeader
        title={t('pages.simulations.title')}
        description={t('pages.simulations.description')}
        hideTitle
      />

      <PageGuide
        title={t('guide.howToRead')}
        steps={[tp('sim.guide.step1'), tp('sim.guide.step2'), tp('sim.guide.step3')]}
      />

      <PageHero
        label={tp('sim.hero.label')}
        value={roles.length}
        hint={tp('sim.hero.hint')}
        metrics={<PageHeroMetric label={tp('sim.hero.roles')} value={roles.length} />}
      />

      <div className="grid gap-6 lg:grid-cols-5">
        <PageSection title={tp('sim.scenario')} className="lg:col-span-2">
          <form onSubmit={onSubmit} className="flex flex-col gap-4">
            <div className={formRowClass}>
              <label className={labelClass}>
                {tp('sim.desiredStart')}
                <input
                  type="date"
                  value={desiredStart}
                  onChange={(e) => setDesiredStart(e.target.value)}
                  className={inputClass}
                  required
                />
              </label>
              <label className={labelClass}>
                {tp('sim.durationMonths')}
                <input
                  type="number"
                  min={1}
                  max={36}
                  value={durationMonths}
                  onChange={(e) => setDurationMonths(e.target.value)}
                  className={`w-24 ${inputClass}`}
                />
              </label>
            </div>

            <div>
              <div className="mb-2 flex items-center justify-between">
                <p className="text-xs ias-text-muted">{tp('sim.requiredRoles')}</p>
                <button
                  type="button"
                  onClick={() => setRoles((prev) => [...prev, newRoleRow()])}
                  className="text-xs ias-link"
                >
                  {tp('sim.addRole')}
                </button>
              </div>
              <div className="space-y-3">
                {roles.map((r) => (
                  <div key={r.id} className="rounded-lg ias-list-row p-3">
                    <div className="flex flex-wrap items-end gap-2">
                      <label className={labelClass}>
                        {t('common.role')}
                        <input
                          value={r.role}
                          onChange={(e) => updateRole(r.id, { role: e.target.value })}
                          className={inputClass}
                        />
                      </label>
                      <label className={labelClass}>
                        {t('common.seniority')}
                        <select
                          value={r.seniority}
                          onChange={(e) => updateRole(r.id, { seniority: e.target.value })}
                          className={inputClass}
                        >
                          {SENIORITY_OPTIONS.map((s) => (
                            <option key={s} value={s}>
                              {s}
                            </option>
                          ))}
                        </select>
                      </label>
                      <label className={labelClass}>
                        {t('common.dedication')}
                        <input
                          type="number"
                          min={1}
                          max={100}
                          value={r.dedication}
                          onChange={(e) => updateRole(r.id, { dedication: e.target.value })}
                          className={`w-16 ${inputClass}`}
                        />
                      </label>
                      <label className={labelClass}>
                        {tp('common.quantity')}
                        <input
                          type="number"
                          min={1}
                          max={20}
                          value={r.quantity}
                          onChange={(e) => updateRole(r.id, { quantity: e.target.value })}
                          className={`w-16 ${inputClass}`}
                        />
                      </label>
                      {roles.length > 1 && (
                        <button
                          type="button"
                          onClick={() => setRoles((prev) => prev.filter((x) => x.id !== r.id))}
                          className="mb-0.5 text-xs ias-text-danger hover:ias-text-danger"
                        >
                          {t('common.remove')}
                        </button>
                      )}
                    </div>
                    {skills.data && skills.data.items.length > 0 && (
                      <div className="mt-3">
                        <p className="mb-1 text-xs ias-text-subtle">{t('form.needs.requiredSkills')}</p>
                        <div className="flex flex-wrap gap-2">
                          {skills.data.items.map((s) => (
                            <label key={s.id} className={`${chipClass} cursor-pointer gap-1.5`}>
                              <input
                                type="checkbox"
                                checked={r.requiredSkillIds.includes(s.id)}
                                onChange={() => toggleRoleSkill(r.id, s.id)}
                                className="rounded border-[var(--ias-border-strong)]"
                              />
                              {s.name}
                            </label>
                          ))}
                        </div>
                      </div>
                    )}
                  </div>
                ))}
              </div>
            </div>

            <button
              type="submit"
              disabled={!isTenantValid || simulate.isPending}
              className={`w-fit ${btnPrimaryClass}`}
            >
              {tp('sim.simulate')}
            </button>
          </form>
          {formError && (
            <div className="mt-3">
              <Alert message={formError} />
            </div>
          )}
        </PageSection>

        <div className="lg:col-span-3">
          {simulate.isPending && <p className="ias-text-subtle">{tp('sim.analyzing')}</p>}

          {!result && !simulate.isPending && (
            <Card className="flex min-h-[280px] items-center justify-center border-dashed">
              <p className="max-w-sm text-center text-sm ias-text-subtle">
                {tp('sim.emptyPrompt')}{' '}
                <strong className="ias-text-muted">{tp('sim.emptyPromptBold')}</strong>{' '}
                {t('common.to').toLowerCase()} {tp('sim.eligibleCandidates').toLowerCase()}.
              </p>
            </Card>
          )}

          {result && (
            <div className="space-y-4">
              <Card
                className={
                  result.feasibleAtDesiredStart
                    ? 'ias-border-success ias-bg-success-subtle'
                    : 'ias-border-warning ias-bg-warning-subtle'
                }
              >
                <div className="flex flex-wrap items-start justify-between gap-4">
                  <div>
                    <p className="text-xs uppercase tracking-wide ias-text-subtle">{tp('sim.verdict')}</p>
                    <h3 className="mt-1 text-xl font-semibold ias-text">
                      {result.feasibleAtDesiredStart ? tp('sim.feasible') : tp('sim.notFeasible')}
                    </h3>
                    <p className="mt-2 text-sm ias-text-muted">
                      {result.feasibleAtDesiredStart ? (
                        <>{tp('sim.canStartOn', { date: result.desiredStartDate })}</>
                      ) : (
                        <>
                          {tp('sim.notFeasibleOn', { date: result.desiredStartDate })}
                          {result.earliestFeasibleStart && (
                            <>
                              {tp('sim.earliestEstimate', {
                                date: result.earliestFeasibleStart,
                                weeks: result.weeksScanned,
                              })}
                            </>
                          )}
                        </>
                      )}
                    </p>
                  </div>
                  <div className="grid grid-cols-1 gap-3 text-center sm:grid-cols-3">
                    <div className="rounded-lg ias-panel-inset p-3 px-3 py-2">
                      <p className="text-lg font-semibold ias-text">{result.activePeopleCount}</p>
                      <p className="text-xs ias-text-subtle">{tp('sim.activePeople')}</p>
                    </div>
                    <div className="rounded-lg ias-panel-inset p-3 px-3 py-2">
                      <p className="text-lg font-semibold ias-text-success">{result.benchAtDesiredStart}</p>
                      <p className="text-xs ias-text-subtle">{tp('sim.onBench')}</p>
                    </div>
                    <div className="rounded-lg ias-panel-inset p-3 px-3 py-2">
                      <p className={`text-lg font-semibold ${totalGap > 0 ? 'ias-text-warning' : 'ias-text'}`}>
                        {totalGap > 0 ? totalGap : result.totalHeadcountRequired}
                      </p>
                      <p className="text-xs ias-text-subtle">
                        {totalGap > 0 ? tp('sim.gapRoles') : tp('sim.requestedRoles')}
                      </p>
                    </div>
                  </div>
                </div>
                <p className="mt-3 text-xs ias-text-subtle">
                  {tp('sim.simulatedPeriod', {
                    from: result.desiredStartDate,
                    to: result.simulatedEndDate,
                  })}
                </p>
              </Card>

              <div className="space-y-3">
                {result.roles.map((role, idx) => (
                  <Card key={`${role.role}-${idx}`}>
                    <div className="flex flex-wrap items-baseline justify-between gap-2">
                      <div>
                        <h4 className="font-semibold ias-text">{role.role}</h4>
                        <p className="text-xs ias-text-subtle">
                          {role.expectedSeniority ?? tp('sim.anySeniority')} ·{' '}
                          {tp('sim.dedicationShort', { percent: role.dedicationPercent })}
                        </p>
                      </div>
                      <span
                        className={
                          role.satisfiedAtDesiredStart ? 'ias-chip-success' : 'ias-chip-warning'
                        }
                      >
                        {role.satisfiedAtDesiredStart ? tp('sim.ok') : tp('sim.bottleneck')}
                      </span>
                    </div>

                    <RoleProgressBar role={role} />

                    {role.eligibleCandidates.length > 0 ? (
                      <div className="mt-3">
                        <p className="mb-2 text-xs ias-text-subtle">{tp('sim.eligibleCandidates')}</p>
                        <div className="flex flex-wrap gap-2">
                          {role.eligibleCandidates.map((c) => (
                            <span
                              key={c.personId}
                              className="inline-flex items-center gap-1.5 rounded-full bg-[var(--ias-surface-muted)] px-3 py-1 text-xs ias-text-muted ring-1 ring-[var(--ias-border)]"
                            >
                              {c.personName}
                              {c.seniority && (
                                <span className="ias-text-subtle">{c.seniority}</span>
                              )}
                              <span className="ias-link">{formatPercent(c.minAvailablePercent)}</span>
                            </span>
                          ))}
                        </div>
                      </div>
                    ) : (
                      <p className="mt-3 text-sm ias-text-warning/80">{tp('sim.noEligible')}</p>
                    )}
                  </Card>
                ))}
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
