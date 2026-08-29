import { useQuery } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { Alert } from '../components/Alert'
import { Card } from '../components/Card'
import { PageGuide } from '../components/page/PageGuide'
import { PageHero, PageHeroMetric } from '../components/page/PageHero'
import { useLocale } from '../context/LocaleContext'
import { useApiOptions, useSettings } from '../context/SettingsContext'
import { api } from '../lib/api'
import { defaultPeriod } from '../lib/dates'
import { isHiddenNavPath } from '../lib/nav'
import type { AllocationConflict, FinancialOverview, Paged, UnderstaffedProject } from '../lib/types'

export function DashboardPage() {
  const { isTenantValid, hiddenOptionsEnabled } = useSettings()
  const { t } = useLocale()
  const opts = useApiOptions()

  const period = defaultPeriod()
  const periodParams = `from=${period.from}&to=${period.to}`

  const counts = useQuery({
    queryKey: ['dashboard-counts', opts.tenantId],
    enabled: isTenantValid,
    queryFn: async () => {
      const [people, projects, allocations, conflicts, understaffed, financials] =
        await Promise.all([
          api.get<Paged<unknown>>('/api/v1/people?page=1&pageSize=1', opts),
          api.get<Paged<unknown>>('/api/v1/projects?page=1&pageSize=1', opts),
          api.get<Paged<unknown>>('/api/v1/allocations?page=1&pageSize=1', opts),
          api.get<{ items: AllocationConflict[] }>('/api/v1/allocations/conflicts', opts),
          api.get<{ items: UnderstaffedProject[] }>(
            '/api/v1/capacity/projects-understaffed',
            opts,
          ),
          api.get<FinancialOverview>(
            `/api/v1/financials/overview?${periodParams}&marginAlertThreshold=15`,
            opts,
          ),
        ])
      return {
        people: people.totalCount,
        projects: projects.totalCount,
        allocations: allocations.totalCount,
        conflicts: conflicts.items.length,
        understaffed: understaffed.items.length,
        marginAlerts: financials.lowMarginAlerts.length,
      }
    },
  })

  const attentionTotal =
    (counts.data?.conflicts ?? 0) +
    (counts.data?.understaffed ?? 0) +
    (counts.data?.marginAlerts ?? 0)

  const quickLinks = [
    { to: '/people', label: t('nav.people') },
    { to: '/allocation-needs', label: t('nav.needs') },
    { to: '/capacity', label: t('nav.capacity') },
    { to: '/matching', label: t('nav.matching') },
    { to: '/simulations', label: t('nav.simulations') },
    { to: '/financials', label: t('nav.financials') },
  ].filter((link) => hiddenOptionsEnabled || !isHiddenNavPath(link.to))

  return (
    <div>
      <PageGuide
        title={t('guide.howToRead')}
        steps={[
          t('pages.dashboard.guide.step1'),
          t('pages.dashboard.guide.step2'),
          t('pages.dashboard.guide.step3'),
        ]}
      />

      <PageHero
        label={t('pages.dashboard.hero.label')}
        value={counts.isLoading ? '—' : attentionTotal}
        hint={t('pages.dashboard.hero.hint')}
        tone={attentionTotal > 0 ? 'warning' : 'success'}
        alert={
          !counts.isLoading
            ? attentionTotal > 0
              ? t('pages.dashboard.hero.attention')
              : t('pages.dashboard.hero.healthy')
            : undefined
        }
        metrics={
          <>
            <PageHeroMetric
              label={t('pages.dashboard.people')}
              value={counts.data?.people ?? '—'}
            />
            <PageHeroMetric
              label={t('pages.dashboard.projects')}
              value={counts.data?.projects ?? '—'}
            />
            <PageHeroMetric
              label={t('pages.dashboard.allocations')}
              value={counts.data?.allocations ?? '—'}
            />
            <PageHeroMetric
              label={t('pages.dashboard.conflicts')}
              value={counts.data?.conflicts ?? '—'}
              valueClassName={(counts.data?.conflicts ?? 0) > 0 ? 'ias-text-warning' : ''}
            />
          </>
        }
      />

      <div className="mb-4 grid gap-4 lg:grid-cols-12">
        <div className="grid gap-3 sm:grid-cols-2 lg:col-span-7">
          <Card className={counts.data?.understaffed ? 'ias-border-warning' : ''}>
            <p className="ias-stat-label">{t('pages.dashboard.understaffed')}</p>
            <p
              className={`ias-stat-value ${(counts.data?.understaffed ?? 0) > 0 ? 'ias-text-warning' : 'ias-text'}`}
            >
              {counts.isLoading ? '—' : (counts.data?.understaffed ?? 0)}
            </p>
          </Card>
          <Card className={counts.data?.marginAlerts ? 'ias-border-warning' : ''}>
            <p className="ias-stat-label">{t('pages.dashboard.marginAlerts')}</p>
            <p
              className={`ias-stat-value ${(counts.data?.marginAlerts ?? 0) > 0 ? 'ias-text-warning' : 'ias-text'}`}
            >
              {counts.isLoading ? '—' : (counts.data?.marginAlerts ?? 0)}
            </p>
          </Card>
        </div>

        <aside className="ias-spotlight-card lg:col-span-5">
          <p className="ias-spotlight-label">{t('pages.dashboard.layers')}</p>
          <h2 className="ias-spotlight-title">{t('nav.group.allocation')}</h2>
          <p className="mt-2 text-sm leading-relaxed text-white/65">
            {t('pages.dashboard.quickAccessHint')}
          </p>
          <nav className="mt-5 flex flex-col" aria-label={t('pages.dashboard.layers')}>
            {quickLinks.map((link) => (
              <Link key={link.to} to={link.to} className="ias-spotlight-link">
                {link.label}
              </Link>
            ))}
          </nav>
        </aside>
      </div>

      {counts.isError && isTenantValid && (
        <Alert message={(counts.error as Error).message} />
      )}
    </div>
  )
}
