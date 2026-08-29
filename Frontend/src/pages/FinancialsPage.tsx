import { useMutation, useQuery } from '@tanstack/react-query'
import { useState, type FormEvent } from 'react'
import { Alert } from '../components/Alert'
import { Card } from '../components/Card'
import { LoadingState } from '../components/LoadingState'
import { PageHeader } from '../components/PageHeader'
import { PeriodFilter } from '../components/PeriodFilter'
import { PageGuide } from '../components/page/PageGuide'
import { PageHero, PageHeroMetric } from '../components/page/PageHero'
import { PageSection } from '../components/page/PageSection'
import { CapacityDonutChart } from '../components/capacity/CapacityCharts'
import { CAPACITY_CHART_COLORS } from '../components/capacity/chartTheme'
import { StatusBadge } from '../components/StatusBadge'
import { useDomainLabels, useLocale } from '../context/LocaleContext'
import { useApiOptions, useSettings } from '../context/SettingsContext'
import { LIST_PAGE_SIZE } from '../lib/constants'
import { api } from '../lib/api'
import { getErrorMessage } from '../lib/errors'
import { defaultPeriod, formatCurrency, formatPercent } from '../lib/dates'
import { btnPrimaryClass, btnSegmentActiveClass, btnSegmentInactiveClass, inputClass } from '../lib/ui'
import type {
  AllocationMarginSimulation,
  BenchCost,
  FinancialOverview,
  Profitability,
  Paged,
  PersonListItem,
  ProjectFinancials,
  ProjectListItem,
} from '../lib/types'

function todayIso(): string {
  return new Date().toISOString().slice(0, 10)
}

export function FinancialsPage() {
  const { t, tp } = useLocale()
  const { projectStatus: projectStatusLabels } = useDomainLabels()
  const { isTenantValid } = useSettings()
  const opts = useApiOptions()
  const [period, setPeriod] = useState(defaultPeriod)
  const [marginThreshold, setMarginThreshold] = useState(15)
  const [selectedProjectId, setSelectedProjectId] = useState('')
  const [simProjectId, setSimProjectId] = useState('')
  const [simPersonId, setSimPersonId] = useState('')
  const [simRole, setSimRole] = useState('Backend')
  const [simDedication, setSimDedication] = useState('50')
  const [simStart, setSimStart] = useState(todayIso())
  const [simEnd, setSimEnd] = useState(todayIso())
  const [simError, setSimError] = useState<string | null>(null)
  const [marginSimResult, setMarginSimResult] = useState<AllocationMarginSimulation | null>(null)
  const [profitGroupBy, setProfitGroupBy] = useState<'Client' | 'ProjectType'>('Client')

  const params = `from=${period.from}&to=${period.to}&marginAlertThreshold=${marginThreshold}`
  const periodOnly = `from=${period.from}&to=${period.to}`

  const overview = useQuery({
    queryKey: ['financials-overview', opts.tenantId, period, marginThreshold],
    enabled: isTenantValid,
    queryFn: () => api.get<FinancialOverview>(`/api/v1/financials/overview?${params}`, opts),
  })

  const projects = useQuery({
    queryKey: ['projects-list', opts.tenantId],
    enabled: isTenantValid,
    queryFn: () => api.get<Paged<ProjectListItem>>('/api/v1/projects?page=1&pageSize=100', opts),
  })

  const people = useQuery({
    queryKey: ['people', opts.tenantId],
    enabled: isTenantValid,
    queryFn: () =>
      api.get<Paged<PersonListItem>>(`/api/v1/people?page=1&pageSize=${LIST_PAGE_SIZE}`, opts),
  })

  const profitability = useQuery({
    queryKey: ['financials-profitability', opts.tenantId, period, marginThreshold, profitGroupBy],
    enabled: isTenantValid,
    queryFn: () =>
      api.get<Profitability>(
        `/api/v1/financials/profitability?${params}&groupBy=${profitGroupBy}`,
        opts,
      ),
  })

  const bench = useQuery({
    queryKey: ['financials-bench', opts.tenantId, period],
    enabled: isTenantValid,
    queryFn: () =>
      api.get<BenchCost>(`/api/v1/financials/bench?${periodOnly}&minAvailablePercent=50`, opts),
  })

  const marginSim = useMutation({
    mutationFn: (body: Record<string, unknown>) =>
      api.post<AllocationMarginSimulation>('/api/v1/simulations/allocation-margin', body, opts),
    onSuccess: (data) => {
      setSimError(null)
      setMarginSimResult(data)
    },
    onError: (err: Error) => {
      setSimError(getErrorMessage(err))
      setMarginSimResult(null)
    },
  })

  const detail = useQuery({
    queryKey: ['project-financials', opts.tenantId, selectedProjectId, period, marginThreshold],
    enabled: isTenantValid && !!selectedProjectId,
    queryFn: () =>
      api.get<ProjectFinancials>(
        `/api/v1/projects/${selectedProjectId}/financials?${params}`,
        opts,
      ),
  })

  return (
    <div>
      <PageHeader
        title={t('pages.financials.title')}
        description={t('pages.financials.description')}
        hideTitle
      >
        <div className="flex flex-wrap items-end gap-4">
          <PeriodFilter
            from={period.from}
            to={period.to}
            onFromChange={(from) => setPeriod((p) => ({ ...p, from }))}
            onToChange={(to) => setPeriod((p) => ({ ...p, to }))}
          />
          <label className="flex flex-col gap-1 text-xs ias-text-muted">
            {tp('financials.marginAlertThreshold')}
            <input
              type="number"
              min={0}
              max={100}
              value={marginThreshold}
              onChange={(e) => setMarginThreshold(Number(e.target.value))}
              className={`w-20 ${inputClass}`}
            />
          </label>
        </div>
      </PageHeader>

      {overview.isLoading && <LoadingState />}
      {overview.isError && <Alert message={getErrorMessage(overview.error)} />}

      {overview.isSuccess && (
        <>
          <PageGuide
            title={t('guide.howToRead')}
            steps={[
              tp('financials.guide.step1'),
              tp('financials.guide.step2'),
              tp('financials.guide.step3'),
            ]}
          />

          <PageHero
            label={tp('financials.hero.label')}
            value={formatPercent(overview.data.avgMarginPercent)}
            hint={tp('financials.hero.hint')}
            tone={
              (overview.data.avgMarginPercent ?? 0) < marginThreshold
                ? 'warning'
                : 'success'
            }
            metrics={
              <>
                <PageHeroMetric
                  label={t('financials.totalRevenue')}
                  value={formatCurrency(overview.data.totalRevenue)}
                />
                <PageHeroMetric
                  label={t('financials.totalCost')}
                  value={formatCurrency(overview.data.totalCost)}
                />
                <PageHeroMetric
                  label={t('financials.marginAlerts')}
                  value={overview.data.lowMarginAlerts.length}
                  valueClassName={
                    overview.data.lowMarginAlerts.length > 0 ? 'ias-text-warning' : ''
                  }
                />
                <PageHeroMetric
                  label={tp('financials.hero.benchCost')}
                  value={bench.data ? formatCurrency(bench.data.totalBenchCost) : '—'}
                />
              </>
            }
          />

          {(overview.data.totalRevenue ?? 0) > 0 && (
            <div className="mb-6">
              <PageSection
                title={tp('financials.donut.title')}
                subtitle={tp('financials.donut.subtitle')}
              >
                <CapacityDonutChart
                  centerValue={formatCurrency(overview.data.totalRevenue)}
                  centerLabel={t('financials.totalRevenue')}
                  slices={[
                    {
                      id: 'allocated' as const,
                      value: overview.data.totalCost,
                      percent:
                        Math.round(
                          (overview.data.totalCost / (overview.data.totalRevenue ?? 1)) * 1000,
                        ) / 10,
                    },
                    {
                      id: 'available' as const,
                      value: overview.data.totalMargin ?? 0,
                      percent:
                        Math.round(
                          ((overview.data.totalMargin ?? 0) /
                            (overview.data.totalRevenue ?? 1)) *
                            1000,
                        ) / 10,
                    },
                  ].filter((slice) => slice.value > 0)}
                  labels={[
                    {
                      id: 'allocated',
                      label: tp('financials.donut.cost'),
                      color: CAPACITY_CHART_COLORS.allocated,
                    },
                    {
                      id: 'available',
                      label: tp('financials.donut.margin'),
                      color: CAPACITY_CHART_COLORS.available,
                    },
                  ]}
                />
              </PageSection>
            </div>
          )}

          {overview.data.lowMarginAlerts.length > 0 && (
            <Card className="mb-6 ias-border-warning">
              <h3 className="mb-4 text-sm font-semibold ias-text-warning">
                {tp('financials.marginAlertTitle', { threshold: marginThreshold })}
              </h3>
              <ul className="space-y-2">
                {overview.data.lowMarginAlerts.map((a) => (
                  <li
                    key={a.projectId}
                    className="flex flex-wrap justify-between gap-2 rounded-lg ias-list-row px-3 py-2 text-sm"
                  >
                    <span className="ias-text">
                      {a.projectName}{' '}
                      <span className="ias-text-subtle">({a.clientName})</span>
                    </span>
                    <span className="ias-text-warning">
                      {formatPercent(a.marginPercent)} · {tp('financials.costLabel')}{' '}
                      {formatCurrency(a.totalCost)}
                    </span>
                  </li>
                ))}
              </ul>
            </Card>
          )}

          <Card className="mb-6">
            <h3 className="mb-4 text-sm font-semibold ias-text-muted">{tp('financials.projectsSection')}</h3>
            <div className="overflow-x-auto">
              <table className="w-full min-w-[600px] text-left text-sm">
                <thead>
                  <tr className="border-b border-[var(--ias-border)] text-xs uppercase ias-text-subtle">
                    <th className="pb-2 pr-4">{t('common.project')}</th>
                    <th className="pb-2 pr-4">{tp('financials.revenue')}</th>
                    <th className="pb-2 pr-4">{tp('common.cost')}</th>
                    <th className="pb-2 pr-4">{tp('common.margin')}</th>
                    <th className="pb-2">{t('common.status')}</th>
                  </tr>
                </thead>
                <tbody>
                  {overview.data.projects.map((p) => (
                    <tr
                      key={p.projectId}
                      className={`border-b border-[var(--ias-border)]/60 ias-text-muted ${
                        p.isLowMarginAlert ? 'ias-bg-warning-subtle' : ''
                      }`}
                    >
                      <td className="py-2 pr-4">
                        <button
                          type="button"
                          onClick={() => setSelectedProjectId(p.projectId)}
                          className="text-left ias-text hover:ias-link"
                        >
                          {p.projectName}
                        </button>
                      </td>
                      <td className="py-2 pr-4">{formatCurrency(p.estimatedRevenue)}</td>
                      <td className="py-2 pr-4">{formatCurrency(p.totalCost)}</td>
                      <td className="py-2 pr-4">
                        <span className={p.isLowMarginAlert ? 'ias-text-warning' : ''}>
                          {formatPercent(p.marginPercent)}
                        </span>
                      </td>
                      <td className="py-2">
                        <StatusBadge
                          label={projectStatusLabels[p.status]}
                          status={p.status}
                        />
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </Card>

          {profitability.isSuccess && (
            <Card className="mb-6">
              <div className="mb-4 flex flex-wrap items-center justify-between gap-3">
                <h3 className="text-sm font-semibold ias-text-muted">{tp('financials.profitability')}</h3>
                <div className="flex gap-2">
                  <button
                    type="button"
                    onClick={() => setProfitGroupBy('Client')}
                    className={
                      profitGroupBy === 'Client' ? btnSegmentActiveClass : btnSegmentInactiveClass
                    }
                  >
                    {tp('financials.byClient')}
                  </button>
                  <button
                    type="button"
                    onClick={() => setProfitGroupBy('ProjectType')}
                    className={
                      profitGroupBy === 'ProjectType' ? btnSegmentActiveClass : btnSegmentInactiveClass
                    }
                  >
                    {tp('financials.byProjectType')}
                  </button>
                </div>
              </div>
              {profitability.data.groups.length === 0 ? (
                <p className="text-sm ias-text-subtle">{tp('financials.noDataInPeriod')}</p>
              ) : (
                <table className="w-full text-left text-sm">
                  <thead>
                    <tr className="border-b border-[var(--ias-border)] text-xs uppercase ias-text-subtle">
                      <th className="pb-2 pr-4">
                        {profitGroupBy === 'Client' ? t('common.client') : tp('financials.groupType')}
                      </th>
                      <th className="pb-2 pr-4">{tp('financials.projectCount')}</th>
                      <th className="pb-2 pr-4">{tp('financials.revenue')}</th>
                      <th className="pb-2 pr-4">{tp('common.cost')}</th>
                      <th className="pb-2">{tp('common.margin')}</th>
                    </tr>
                  </thead>
                  <tbody>
                    {profitability.data.groups.map((g) => (
                      <tr
                        key={g.groupKey}
                        className={`border-b border-[var(--ias-border)]/60 ias-text-muted ${
                          g.isLowMarginAlert ? 'ias-bg-warning-subtle' : ''
                        }`}
                      >
                        <td className="py-2 pr-4 ias-text">{g.groupKey}</td>
                        <td className="py-2 pr-4">{g.projectCount}</td>
                        <td className="py-2 pr-4">{formatCurrency(g.totalRevenue)}</td>
                        <td className="py-2 pr-4">{formatCurrency(g.totalCost)}</td>
                        <td className="py-2">
                          <span className={g.isLowMarginAlert ? 'ias-text-warning' : ''}>
                            {formatPercent(g.marginPercent)}
                          </span>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              )}
            </Card>
          )}
        </>
      )}

      <Card>
        <label className="mb-4 flex flex-col gap-2 text-sm">
          <span className="ias-text-muted">{tp('financials.projectDetail')}</span>
          <select
            value={selectedProjectId}
            onChange={(e) => setSelectedProjectId(e.target.value)}
            className={inputClass}
          >
            <option value="">{tp('financials.selectProject')}</option>
            {projects.data?.items.map((p) => (
              <option key={p.id} value={p.id}>
                {p.name} ({p.clientName})
              </option>
            ))}
          </select>
        </label>

        {detail.isLoading && selectedProjectId && (
          <p className="text-sm ias-text-subtle">{tp('financials.loadingDetail')}</p>
        )}
        {detail.isError && <Alert message={(detail.error as Error).message} />}

        {detail.isSuccess && (
          <div>
            <div className="mb-4 grid gap-4 sm:grid-cols-3">
              <div>
                <p className="text-xs ias-text-subtle">{tp('financials.estimatedRevenue')}</p>
                <p className="text-lg font-semibold ias-text">
                  {formatCurrency(detail.data.estimatedRevenue)}
                </p>
              </div>
              <div>
                <p className="text-xs ias-text-subtle">{tp('financials.periodCost')}</p>
                <p className="text-lg font-semibold ias-text">
                  {formatCurrency(detail.data.totalCost)}
                </p>
              </div>
              <div>
                <p className="text-xs ias-text-subtle">{tp('common.margin')}</p>
                <p
                  className={`text-lg font-semibold ${
                    detail.data.isLowMarginAlert ? 'ias-text-warning' : 'ias-text-success'
                  }`}
                >
                  {formatCurrency(detail.data.marginAmount)} ({formatPercent(detail.data.marginPercent)})
                </p>
              </div>
            </div>

            {detail.data.allocations.length === 0 ? (
              <p className="text-sm ias-text-subtle">{tp('financials.noAllocationsInPeriod')}</p>
            ) : (
              <table className="w-full text-left text-sm">
                <thead>
                  <tr className="border-b border-[var(--ias-border)] text-xs uppercase ias-text-subtle">
                    <th className="pb-2 pr-4">{tp('common.person')}</th>
                    <th className="pb-2 pr-4">{t('common.role')}</th>
                    <th className="pb-2 pr-4">{tp('common.hours')}</th>
                    <th className="pb-2">{tp('common.cost')}</th>
                  </tr>
                </thead>
                <tbody>
                  {detail.data.allocations.map((a) => (
                    <tr key={a.allocationId} className="border-b border-[var(--ias-border)]/60 ias-text-muted">
                      <td className="py-2 pr-4 ias-text">{a.personName}</td>
                      <td className="py-2 pr-4">
                        {a.role} · {a.dedicationPercent}%
                      </td>
                      <td className="py-2 pr-4">{a.totalHours.toFixed(0)}h</td>
                      <td className="py-2">{formatCurrency(a.totalCost)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </div>
        )}
      </Card>

      {bench.isSuccess && (
        <Card className="mt-6">
          <h3 className="mb-4 text-sm font-semibold ias-text-muted">{tp('financials.benchCost')}</h3>
          <p className="mb-4 text-sm ias-text-muted">
            {tp('financials.benchCostDesc')}{' '}
            <span className="ias-text">{formatCurrency(bench.data.totalBenchCost)}</span>
            {' · '}
            {bench.data.totalBenchHours.toFixed(0)}h no período
          </p>
          {bench.data.people.length === 0 ? (
            <p className="text-sm ias-text-subtle">{tp('financials.nobodyOnBench')}</p>
          ) : (
            <ul className="space-y-2">
              {bench.data.people.slice(0, 8).map((p) => (
                <li
                  key={p.personId}
                  className="flex justify-between rounded-lg ias-list-row px-3 py-2 text-sm"
                >
                  <span className="ias-text">{p.personName}</span>
                  <span className="ias-text-muted">
                    {p.benchHours.toFixed(0)}h · {formatCurrency(p.benchCost)}
                  </span>
                </li>
              ))}
            </ul>
          )}
        </Card>
      )}

      <Card className="mt-6">
        <h3 className="mb-4 text-sm font-semibold ias-text-muted">{tp('financials.marginSimulation')}</h3>
        <form
          onSubmit={(e: FormEvent) => {
            e.preventDefault()
            marginSim.mutate({
              projectId: simProjectId,
              personId: simPersonId,
              role: simRole.trim(),
              dedicationPercent: Number(simDedication),
              startDate: simStart,
              endDate: simEnd,
              marginAlertThresholdPercent: marginThreshold,
            })
          }}
          className="flex flex-wrap items-end gap-3"
        >
          <label className="flex flex-col gap-1 text-xs ias-text-muted">
            {t('common.project')}
            <select
              value={simProjectId}
              onChange={(e) => setSimProjectId(e.target.value)}
              className={inputClass}
              required
            >
              <option value="">{t('common.select')}</option>
              {projects.data?.items.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.name}
                </option>
              ))}
            </select>
          </label>
          <label className="flex flex-col gap-1 text-xs ias-text-muted">
            {tp('common.person')}
            <select
              value={simPersonId}
              onChange={(e) => setSimPersonId(e.target.value)}
              className={inputClass}
              required
            >
              <option value="">{t('common.select')}</option>
              {people.data?.items.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.name}
                </option>
              ))}
            </select>
          </label>
          <label className="flex flex-col gap-1 text-xs ias-text-muted">
            {t('common.role')}
            <input value={simRole} onChange={(e) => setSimRole(e.target.value)} className={inputClass} />
          </label>
          <label className="flex flex-col gap-1 text-xs ias-text-muted">
            {t('common.dedication')}
            <input
              type="number"
              min={1}
              max={100}
              value={simDedication}
              onChange={(e) => setSimDedication(e.target.value)}
              className={`w-16 ${inputClass}`}
            />
          </label>
          <label className="flex flex-col gap-1 text-xs ias-text-muted">
            {t('common.startDate')}
            <input
              type="date"
              value={simStart}
              onChange={(e) => setSimStart(e.target.value)}
              className={inputClass}
            />
          </label>
          <label className="flex flex-col gap-1 text-xs ias-text-muted">
            {t('common.endDate')}
            <input
              type="date"
              value={simEnd}
              onChange={(e) => setSimEnd(e.target.value)}
              className={inputClass}
            />
          </label>
          <button
            type="submit"
            disabled={!isTenantValid || marginSim.isPending}
            className={btnPrimaryClass}
          >
            {tp('common.simulate')}
          </button>
        </form>
        {simError && (
          <div className="mt-3">
            <Alert message={simError} />
          </div>
        )}
        {marginSimResult && (
          <div className="mt-4 grid gap-4 sm:grid-cols-3">
            <div>
              <p className="text-xs ias-text-subtle">{tp('financials.currentMargin')}</p>
              <p className="text-lg font-semibold ias-text">
                {formatPercent(marginSimResult.currentMarginPercent)}
              </p>
            </div>
            <div>
              <p className="text-xs ias-text-subtle">{tp('financials.projectedMargin')}</p>
              <p
                className={`text-lg font-semibold ${
                  marginSimResult.projectedIsLowMarginAlert ? 'ias-text-warning' : 'ias-text-success'
                }`}
              >
                {formatPercent(marginSimResult.projectedMarginPercent)}
              </p>
            </div>
            <div>
              <p className="text-xs ias-text-subtle">{tp('financials.additionalCost')}</p>
              <p className="text-lg font-semibold ias-text">
                {formatCurrency(marginSimResult.simulatedAdditionalCost)}
              </p>
            </div>
          </div>
        )}
      </Card>
    </div>
  )
}
