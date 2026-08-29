import { useQuery } from '@tanstack/react-query'
import { useMemo, useState } from 'react'
import { Alert } from '../components/Alert'
import { Card } from '../components/Card'
import {
  CapacityDonutChart,
  CapacityGapChart,
  CapacityHoursChart,
  CapacityBarList,
  CapacityTeamBars,
  CapacityTrendChart,
} from '../components/capacity/CapacityCharts'
import { CAPACITY_CHART_COLORS } from '../components/capacity/chartTheme'
import { PageGuide } from '../components/page/PageGuide'
import { LoadingState } from '../components/LoadingState'
import { PageHeader } from '../components/PageHeader'
import { PeriodFilter } from '../components/PeriodFilter'
import { SectionTitle } from '../components/SectionTitle'
import { StatusBadge } from '../components/StatusBadge'
import { useDomainLabels, useLocale } from '../context/LocaleContext'
import { useApiOptions, useSettings } from '../context/SettingsContext'
import { api } from '../lib/api'
import {
  donutSlicesFromWeek,
  formatWeekAxisLabel,
  lastWeekOverview,
  peakGapWeek,
  periodAverage,
  utilizationTone,
} from '../lib/capacityMetrics'
import { defaultPeriod, formatPercent } from '../lib/dates'
import { getErrorMessage } from '../lib/errors'
import type {
  BenchPeople,
  CapacityOverview,
  FutureCapacityGaps,
  SkillsOccupation,
  UnderstaffedProject,
} from '../lib/types'

export function CapacityPage() {
  const { t, locale } = useLocale()
  const {
    projectStatus: projectStatusLabels,
    allocationNeedStatus: allocationNeedStatusLabels,
  } = useDomainLabels()
  const { isTenantValid } = useSettings()
  const opts = useApiOptions()
  const [period, setPeriod] = useState(defaultPeriod)

  const params = `from=${period.from}&to=${period.to}`

  const overview = useQuery({
    queryKey: ['capacity-overview', opts.tenantId, period],
    enabled: isTenantValid,
    queryFn: () => api.get<CapacityOverview>(`/api/v1/capacity/overview?${params}`, opts),
  })

  const skills = useQuery({
    queryKey: ['capacity-skills', opts.tenantId, period],
    enabled: isTenantValid,
    queryFn: () =>
      api.get<SkillsOccupation>(`/api/v1/capacity/skills-occupation?${params}`, opts),
  })

  const bench = useQuery({
    queryKey: ['capacity-bench', opts.tenantId, period],
    enabled: isTenantValid,
    queryFn: () =>
      api.get<BenchPeople>(`/api/v1/capacity/bench?${params}&minAvailablePercent=50`, opts),
  })

  const understaffed = useQuery({
    queryKey: ['capacity-understaffed', opts.tenantId],
    enabled: isTenantValid,
    queryFn: () =>
      api.get<{ items: UnderstaffedProject[] }>('/api/v1/capacity/projects-understaffed', opts),
  })

  const futureGaps = useQuery({
    queryKey: ['capacity-future-gaps', opts.tenantId, period],
    enabled: isTenantValid,
    queryFn: () =>
      api.get<FutureCapacityGaps>(`/api/v1/capacity/future-gaps?${params}`, opts),
  })

  const loading = overview.isLoading || skills.isLoading
  const error =
    overview.error ?? skills.error ?? bench.error ?? understaffed.error ?? futureGaps.error

  const weekLabels = useMemo(
    () =>
      overview.data?.weeks.map((week) => formatWeekAxisLabel(week.weekStart, locale)) ?? [],
    [overview.data?.weeks, locale],
  )

  const gapWeekLabels = useMemo(
    () =>
      futureGaps.data?.weeks.map((week) => formatWeekAxisLabel(week.weekStart, locale)) ?? [],
    [futureGaps.data?.weeks, locale],
  )

  const avgAllocated = overview.data
    ? periodAverage(overview.data.weeks, (week) => week.avgAllocatedPercent)
    : null
  const avgAvailable = overview.data
    ? periodAverage(overview.data.weeks, (week) => week.avgAvailablePercent)
    : null
  const lastWeek = overview.data ? lastWeekOverview(overview.data.weeks) : null
  const donutWeek = lastWeek
  const donutSlices = donutWeek ? donutSlicesFromWeek(donutWeek) : []
  const utilizationClass = `ias-page-hero__value ias-page-hero__value--${utilizationTone(avgAllocated)}`
  const peakGap = futureGaps.data ? peakGapWeek(futureGaps.data.weeks) : null

  const donutLabels = [
    { id: 'allocated' as const, label: t('capacity.donut.allocated'), color: CAPACITY_CHART_COLORS.allocated },
    { id: 'available' as const, label: t('capacity.donut.available'), color: CAPACITY_CHART_COLORS.available },
    {
      id: 'unavailable' as const,
      label: t('capacity.donut.unavailable'),
      color: CAPACITY_CHART_COLORS.unavailable,
    },
  ]

  const topSkills = useMemo(() => {
    if (!skills.data) return []
    return [...skills.data.skills]
      .sort((a, b) => b.avgAllocatedPercent - a.avgAllocatedPercent)
      .slice(0, 8)
  }, [skills.data])

  return (
    <div className="capacity-page">
      <PageHeader
        title={t('pages.capacity.title')}
        description={t('pages.capacity.description')}
        hideTitle
      >
        <PeriodFilter
          from={period.from}
          to={period.to}
          onFromChange={(from) => setPeriod((p) => ({ ...p, from }))}
          onToChange={(to) => setPeriod((p) => ({ ...p, to }))}
        />
      </PageHeader>

      <PageGuide
        title={t('capacity.howItWorks.title')}
        steps={[
          t('capacity.howItWorks.step1'),
          t('capacity.howItWorks.step2'),
          t('capacity.howItWorks.step3'),
        ]}
      />

      {loading && <LoadingState />}
      {error && <Alert message={getErrorMessage(error)} />}

      {overview.isSuccess && (
        <section className="ias-page-hero mb-6">
          <div className="ias-page-hero__main">
            <p className="ias-page-hero__label">{t('capacity.utilizationRate')}</p>
            <p className={utilizationClass}>{formatPercent(avgAllocated)}</p>
            <p className="ias-page-hero__hint">{t('capacity.utilizationHint')}</p>
            {lastWeek && lastWeek.overallocatedPeopleCount > 0 ? (
              <p className="ias-page-hero__alert">
                {t('capacity.overloadWarning', {
                  count: String(lastWeek.overallocatedPeopleCount),
                })}
              </p>
            ) : null}
          </div>

          <div className="ias-page-hero__metrics">
            <div className="ias-page-mini-metric">
              <span className="ias-page-mini-metric__label">{t('capacity.avgAvailable')}</span>
              <span className="ias-page-mini-metric__value ias-text-success">
                {formatPercent(avgAvailable)}
              </span>
            </div>
            <div className="ias-page-mini-metric">
              <span className="ias-page-mini-metric__label">{t('capacity.lastWeekHours')}</span>
              <span className="ias-page-mini-metric__value">
                {lastWeek ? `${lastWeek.totalAllocatedHours.toFixed(0)}h` : '—'}
              </span>
            </div>
            <div className="ias-page-mini-metric">
              <span className="ias-page-mini-metric__label">{t('capacity.benchCount')}</span>
              <span className="ias-page-mini-metric__value ias-text-success">
                {lastWeek?.benchPeopleCount ?? '—'}
              </span>
            </div>
            <div className="ias-page-mini-metric">
              <span className="ias-page-mini-metric__label">{t('capacity.openRoles')}</span>
              <span
                className={`ias-page-mini-metric__value ${
                  (understaffed.data?.items.length ?? 0) > 0 ? 'ias-text-warning' : ''
                }`}
              >
                {understaffed.data?.items.length ?? (understaffed.isLoading ? '—' : 0)}
              </span>
            </div>
          </div>
        </section>
      )}

      {overview.isSuccess && overview.data.weeks.length > 0 && (
        <div className="mb-6 grid gap-6 xl:grid-cols-2">
          <Card>
            <SectionTitle>{t('capacity.donut.title')}</SectionTitle>
            <p className="ias-page-card-subtitle">{t('capacity.donut.subtitle')}</p>
            <CapacityDonutChart
              slices={donutSlices}
              labels={donutLabels}
              centerValue={donutWeek ? `${donutWeek.totalCapacityHours.toFixed(0)}h` : '—'}
              centerLabel={t('capacity.donut.centerLabel')}
            />
          </Card>

          <Card>
            <SectionTitle>{t('capacity.trend.title')}</SectionTitle>
            <p className="ias-page-card-subtitle">{t('capacity.trend.subtitle')}</p>
            <CapacityTrendChart
              weeks={overview.data.weeks}
              weekLabels={weekLabels}
              allocatedLabel={t('capacity.avgAllocated')}
              availableLabel={t('capacity.avgAvailable')}
            />
          </Card>
        </div>
      )}

      {overview.isSuccess && overview.data.weeks.length > 0 && (
        <Card className="mb-6">
          <SectionTitle>{t('capacity.hours.title')}</SectionTitle>
          <p className="ias-page-card-subtitle">{t('capacity.hours.subtitle')}</p>
          <CapacityHoursChart
            weeks={overview.data.weeks}
            weekLabels={weekLabels}
            allocatedLabel={t('capacity.donut.allocated')}
            availableLabel={t('capacity.donut.available')}
            unavailableLabel={t('capacity.donut.unavailable')}
          />
        </Card>
      )}

      {futureGaps.isSuccess && (
        <Card
          className={`mb-6 ${futureGaps.data.peakShortfallPercent > 0 ? 'ias-border-warning' : ''}`}
        >
          <div className="capacity-gaps-header">
            <div>
              <SectionTitle>{t('capacity.gaps.title')}</SectionTitle>
              <p className="ias-page-card-subtitle">{t('capacity.gaps.subtitle')}</p>
            </div>
            <div className="capacity-gaps-kpi">
              <span className="capacity-gaps-kpi__label">{t('capacity.peakShortfall')}</span>
              <span
                className={
                  futureGaps.data.peakShortfallPercent > 0
                    ? 'capacity-gaps-kpi__value ias-text-warning'
                    : 'capacity-gaps-kpi__value ias-text-success'
                }
              >
                {formatPercent(futureGaps.data.peakShortfallPercent)}
              </span>
              {peakGap && peakGap.netShortfallPercent > 0 ? (
                <span className="capacity-gaps-kpi__hint">
                  {t('capacity.gaps.peakInWeek', {
                    week: formatWeekAxisLabel(peakGap.weekStart, locale),
                  })}
                </span>
              ) : null}
            </div>
          </div>

          {futureGaps.data.weeks.length > 0 ? (
            <CapacityGapChart
              weeks={futureGaps.data.weeks}
              weekLabels={gapWeekLabels}
              demandLabel={t('capacity.gaps.demand')}
              supplyLabel={t('capacity.gaps.supply')}
            />
          ) : null}

          {futureGaps.data.openNeeds.length === 0 ? (
            <p className="mt-4 text-sm ias-text-success">{t('capacity.gaps.noNeeds')}</p>
          ) : (
            <ul className="ias-page-list mt-4">
              {futureGaps.data.openNeeds.slice(0, 8).map((need) => (
                <li key={need.needId} className="ias-page-list__item">
                  <div>
                    <p className="ias-page-list__title">
                      {need.projectName} · {need.role}
                    </p>
                    <p className="ias-page-list__meta">
                      {allocationNeedStatusLabels[need.status] ?? need.status}
                    </p>
                  </div>
                  <span className="ias-page-list__gap">
                    {formatPercent(need.gapPercent)}
                  </span>
                </li>
              ))}
            </ul>
          )}
        </Card>
      )}

      <div className="mb-6 grid gap-6 lg:grid-cols-2">
        {overview.isSuccess && overview.data.teams.length > 0 && (
          <Card>
            <SectionTitle>{t('capacity.teams.title')}</SectionTitle>
            <p className="ias-page-card-subtitle">{t('capacity.teams.subtitle')}</p>
            <CapacityTeamBars
              teams={overview.data.teams}
              peopleLabel={t('capacity.peopleCount')}
              noTeamLabel={t('capacity.noTeam')}
            />
          </Card>
        )}

        {skills.isSuccess && (
          <Card>
            <SectionTitle>{t('capacity.skills.title')}</SectionTitle>
            <p className="ias-page-card-subtitle">{t('capacity.skills.subtitle')}</p>
            {topSkills.length === 0 ? (
              <p className="text-sm ias-text-subtle">{t('capacity.skills.empty')}</p>
            ) : (
              <CapacityBarList
                items={topSkills.map((skill) => ({
                  id: skill.skillId,
                  label: skill.skillName,
                  value: skill.avgAllocatedPercent,
                  hint: `${skill.peopleCount} ${t('capacity.peopleCount')} · ${skill.avgAllocatedHours.toFixed(0)}h/sem`,
                }))}
              />
            )}
          </Card>
        )}
      </div>

      <div className="grid gap-6 lg:grid-cols-2">
        {bench.isSuccess && (
          <Card>
            <SectionTitle>{t('capacity.bench.title')}</SectionTitle>
            <p className="ias-page-card-subtitle">{t('capacity.bench.subtitle')}</p>
            {bench.data.people.length === 0 ? (
              <p className="text-sm ias-text-subtle">{t('capacity.bench.empty')}</p>
            ) : (
              <ul className="ias-page-list">
                {bench.data.people.map((person) => (
                  <li key={person.personId} className="ias-page-list__item">
                    <div>
                      <p className="ias-page-list__name">{person.personName}</p>
                      <p className="ias-page-list__meta">
                        {[person.team, person.seniority].filter(Boolean).join(' · ') || '—'}
                      </p>
                    </div>
                    <span className="ias-page-list__badge ias-text-success">
                      {formatPercent(person.minAvailablePercentInPeriod)}{' '}
                      <span className="ias-page-list__badge-hint">
                        {t('capacity.bench.minAvailable')}
                      </span>
                    </span>
                  </li>
                ))}
              </ul>
            )}
          </Card>
        )}

        {understaffed.isSuccess && (
          <Card>
            <SectionTitle>{t('capacity.understaffed.title')}</SectionTitle>
            <p className="ias-page-card-subtitle">{t('capacity.understaffed.subtitle')}</p>
            {understaffed.data.items.length === 0 ? (
              <p className="text-sm ias-text-success">{t('capacity.understaffed.empty')}</p>
            ) : (
              <ul className="ias-page-list">
                {understaffed.data.items.map((project) => (
                  <li key={project.projectId} className="ias-page-list__item">
                    <div>
                      <p className="ias-page-list__name">{project.projectName}</p>
                      <p className="ias-page-list__meta">
                        {t('capacity.understaffed.openRoles', {
                          count: String(project.openNeedsCount),
                        })}{' '}
                        · gap {formatPercent(project.totalGapPercent)}
                      </p>
                    </div>
                    <StatusBadge
                      label={projectStatusLabels[project.status]}
                      status={project.status}
                    />
                  </li>
                ))}
              </ul>
            )}
          </Card>
        )}
      </div>
    </div>
  )
}
