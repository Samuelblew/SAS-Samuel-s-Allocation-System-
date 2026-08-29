import { useQuery } from '@tanstack/react-query'
import { Alert } from '../components/Alert'
import { PageGuide } from '../components/page/PageGuide'
import { PageHero, PageHeroMetric } from '../components/page/PageHero'
import { PageSection } from '../components/page/PageSection'
import { EmptyState } from '../components/EmptyState'
import { LoadingState } from '../components/LoadingState'
import { PageHeader } from '../components/PageHeader'
import { useDomainLabels, useLocale } from '../context/LocaleContext'
import { useApiOptions, useSettings } from '../context/SettingsContext'
import { api } from '../lib/api'
import type { AllocationConflict } from '../lib/types'

export function ConflictsPage() {
  const { t } = useLocale()
  const { allocationStatus: allocationStatusLabels } = useDomainLabels()
  const { isTenantValid } = useSettings()
  const opts = useApiOptions()

  const query = useQuery({
    queryKey: ['conflicts', opts.tenantId],
    enabled: isTenantValid,
    queryFn: () =>
      api.get<{ items: AllocationConflict[] }>('/api/v1/allocations/conflicts', opts),
  })

  const conflictCount = query.data?.items.length ?? 0
  const uniqueWeeks = new Set(query.data?.items.map((item) => item.weekStart)).size

  return (
    <div>
      <PageHeader
        title={t('pages.conflicts.title')}
        description={t('pages.conflicts.description')}
        hideTitle
      />

      <PageGuide
        title={t('guide.howToRead')}
        steps={[
          t('pages.conflicts.guide.step1'),
          t('pages.conflicts.guide.step2'),
          t('pages.conflicts.guide.step3'),
        ]}
      />

      {query.isSuccess && (
        <PageHero
          label={t('pages.conflicts.hero.label')}
          value={conflictCount === 0 ? t('pages.conflicts.hero.healthy') : conflictCount}
          hint={t('pages.conflicts.hero.hint')}
          tone={conflictCount > 0 ? 'warning' : 'success'}
          metrics={
            <PageHeroMetric
              label={t('pages.conflicts.hero.weeks')}
              value={uniqueWeeks}
              valueClassName={uniqueWeeks > 0 ? 'ias-text-warning' : ''}
            />
          }
        />
      )}

      {query.isLoading && <LoadingState />}
      {query.isError && <Alert message={(query.error as Error).message} />}
      {query.isSuccess && query.data.items.length === 0 && (
        <EmptyState message={t('empty.conflicts')} variant="success" />
      )}
      {query.isSuccess && query.data.items.length > 0 && (
        <PageSection title={t('pages.conflicts.title')} subtitle={t('pages.conflicts.hero.hint')}>
          <ul className="ias-page-list">
            {query.data.items.map((conflict) => (
              <li key={`${conflict.personId}-${conflict.weekStart}`} className="ias-page-list__item ias-border-warning">
                <div className="w-full">
                  <div className="flex flex-wrap items-baseline justify-between gap-2">
                    <p className="ias-page-list__name">{conflict.personName}</p>
                    <span className="ias-page-list__badge ias-text-warning">
                      {conflict.weekStart} → {conflict.weekEnd} · {conflict.totalDedicationPercent}%
                    </span>
                  </div>
                  <ul className="mt-3 space-y-2">
                    {conflict.allocations.map((allocation) => (
                      <li
                        key={allocation.allocationId}
                        className="ias-list-row flex flex-wrap justify-between gap-2 rounded-lg px-3 py-2 text-sm"
                      >
                        <span className="ias-text">{allocation.projectName}</span>
                        <span className="ias-text-muted">
                          {allocation.dedicationPercent}% ·{' '}
                          {allocationStatusLabels[allocation.status]} · {allocation.startDate} →{' '}
                          {allocation.endDate}
                        </span>
                      </li>
                    ))}
                  </ul>
                </div>
              </li>
            ))}
          </ul>
        </PageSection>
      )}
    </div>
  )
}
