import { zodResolver } from '@hookform/resolvers/zod'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useEffect, useMemo, useState } from 'react'
import { useForm } from 'react-hook-form'
import { Link, useSearchParams } from 'react-router-dom'
import { Alert } from '../components/Alert'
import { DataTable, DataTableBody, DataTableHead } from '../components/DataTable'
import { EmptyState } from '../components/EmptyState'
import { FormField } from '../components/forms/FormField'
import { LoadingState } from '../components/LoadingState'
import { PageHeader } from '../components/PageHeader'
import { PageGuide } from '../components/page/PageGuide'
import { PageHero, PageHeroMetric } from '../components/page/PageHero'
import { PageSection } from '../components/page/PageSection'
import { StatusBadge } from '../components/StatusBadge'
import { TableActions } from '../components/TableActions'
import {
  TableCellChip,
  TableCellIdentity,
  TableCellMetric,
  TableCellPeriod,
} from '../components/table/TableCells'
import { useDomainLabels, useLocale } from '../context/LocaleContext'
import { useApiOptions, useSettings } from '../context/SettingsContext'
import { LIST_PAGE_SIZE } from '../lib/constants'
import { api } from '../lib/api'
import {
  filterActiveOverlapping,
  findFirstWeeklyOverload,
  peakWeeklyLoad,
  toAllocationLoadItem,
} from '../lib/allocationLoad'
import { getErrorMessage } from '../lib/errors'
import {
  allocationFormSchema,
  toAllocationPayload,
  type AllocationFormValues,
} from '../lib/schemas/allocation'
import { btnGhostClass, btnPrimaryClass, formRowClass, inputClass } from '../lib/ui'
import { dedicationTone } from '../lib/tableDisplay'
import type {
  Allocation,
  AllocationListItem,
  Paged,
  PersonListItem,
  ProjectListItem,
} from '../lib/types'

function todayIso(): string {
  return new Date().toISOString().slice(0, 10)
}

const defaultValues: AllocationFormValues = {
  personId: '',
  projectId: '',
  role: 'Consultor',
  dedicationPercent: 50,
  startDate: todayIso(),
  endDate: todayIso(),
  status: 'Planned',
}

export function AllocationsPage() {
  const { t, tp } = useLocale()
  const { allocationStatus: allocationStatusLabels } = useDomainLabels()
  const { isTenantValid } = useSettings()
  const opts = useApiOptions()
  const qc = useQueryClient()
  const [searchParams] = useSearchParams()
  const prefilledFromMatching =
    searchParams.get('from') === 'matching' || !!searchParams.get('personId')

  const [editingId, setEditingId] = useState<string | null>(null)
  const [formError, setFormError] = useState<string | null>(null)

  const form = useForm<AllocationFormValues>({
    resolver: zodResolver(allocationFormSchema),
    defaultValues,
  })

  const watchedPersonId = form.watch('personId')
  const watchedStartDate = form.watch('startDate')
  const watchedEndDate = form.watch('endDate')
  const watchedDedication = form.watch('dedicationPercent')

  const people = useQuery({
    queryKey: ['people', opts.tenantId],
    enabled: isTenantValid,
    queryFn: () =>
      api.get<Paged<PersonListItem>>(`/api/v1/people?page=1&pageSize=${LIST_PAGE_SIZE}`, opts),
  })

  const projects = useQuery({
    queryKey: ['projects', opts.tenantId],
    enabled: isTenantValid,
    queryFn: () =>
      api.get<Paged<ProjectListItem>>(`/api/v1/projects?page=1&pageSize=${LIST_PAGE_SIZE}`, opts),
  })

  const allocations = useQuery({
    queryKey: ['allocations', opts.tenantId],
    enabled: isTenantValid,
    queryFn: () =>
      api.get<Paged<AllocationListItem>>(`/api/v1/allocations?page=1&pageSize=${LIST_PAGE_SIZE}`, opts),
  })

  const personAllocations = useQuery({
    queryKey: ['allocations', opts.tenantId, 'person', watchedPersonId],
    enabled: isTenantValid && !!watchedPersonId,
    queryFn: () =>
      api.get<Paged<AllocationListItem>>(
        `/api/v1/allocations?personId=${watchedPersonId}&page=1&pageSize=${LIST_PAGE_SIZE}`,
        opts,
      ),
  })

  const editingAllocation = useQuery({
    queryKey: ['allocation', opts.tenantId, editingId],
    enabled: isTenantValid && !!editingId,
    queryFn: () => api.get<Allocation>(`/api/v1/allocations/${editingId}`, opts),
  })

  useEffect(() => {
    if (!editingAllocation.data) return
    const a = editingAllocation.data
    form.reset({
      personId: a.personId,
      projectId: a.projectId,
      role: a.role,
      dedicationPercent: a.dedicationPercent,
      startDate: a.startDate,
      endDate: a.endDate,
      status: a.status,
    })
  }, [editingAllocation.data, form])

  useEffect(() => {
    if (editingId) return
    const p = searchParams.get('personId')
    const proj = searchParams.get('projectId')
    const r = searchParams.get('role')
    const ded = searchParams.get('dedication')
    const start = searchParams.get('startDate')
    const end = searchParams.get('endDate')
    if (!p && !proj && !r && !ded && !start && !end) return
    form.reset({
      ...defaultValues,
      personId: p ?? '',
      projectId: proj ?? '',
      role: r ?? defaultValues.role,
      dedicationPercent: ded ? Number(ded) : defaultValues.dedicationPercent,
      startDate: start ?? defaultValues.startDate,
      endDate: end ?? defaultValues.endDate,
    })
  }, [searchParams, editingId, form])

  function resetForm() {
    form.reset(defaultValues)
    setEditingId(null)
    setFormError(null)
  }

  const invalidateRelated = () => {
    void qc.invalidateQueries({ queryKey: ['allocations'] })
    void qc.invalidateQueries({ queryKey: ['allocation'] })
    void qc.invalidateQueries({ queryKey: ['dashboard-counts'] })
    void qc.invalidateQueries({ queryKey: ['conflicts'] })
    void qc.invalidateQueries({ queryKey: ['capacity'] })
  }

  const create = useMutation({
    mutationFn: (body: ReturnType<typeof toAllocationPayload>) =>
      api.post<AllocationListItem>('/api/v1/allocations', body, opts),
    onSuccess: () => {
      resetForm()
      invalidateRelated()
    },
    onError: (err: Error) => setFormError(getErrorMessage(err)),
  })

  const update = useMutation({
    mutationFn: ({ id, body }: { id: string; body: ReturnType<typeof toAllocationPayload> }) =>
      api.put<Allocation>(`/api/v1/allocations/${id}`, body, opts),
    onSuccess: () => {
      resetForm()
      invalidateRelated()
    },
    onError: (err: Error) => setFormError(getErrorMessage(err)),
  })

  const remove = useMutation({
    mutationFn: (id: string) => api.delete(`/api/v1/allocations/${id}`, opts),
    onSuccess: () => {
      if (editingId) resetForm()
      invalidateRelated()
    },
    onError: (err: Error) => setFormError(getErrorMessage(err)),
  })

  const onSubmit = form.handleSubmit((values) => {
    const body = toAllocationPayload(values)
    if (editingId) {
      update.mutate({ id: editingId, body })
    } else {
      create.mutate(body)
    }
  })

  const workloadItems = useMemo(
    () => (personAllocations.data?.items ?? []).map(toAllocationLoadItem),
    [personAllocations.data],
  )

  const overlappingAllocations = useMemo(() => {
    if (!watchedStartDate || !watchedEndDate) return []
    return filterActiveOverlapping(workloadItems, watchedStartDate, watchedEndDate, editingId)
  }, [workloadItems, watchedStartDate, watchedEndDate, editingId])

  const peakLoad = useMemo(() => {
    if (!watchedStartDate || !watchedEndDate) return null
    return peakWeeklyLoad(watchedStartDate, watchedEndDate, workloadItems, editingId)
  }, [workloadItems, watchedStartDate, watchedEndDate, editingId])

  const projectedOverload = useMemo(() => {
    if (!watchedStartDate || !watchedEndDate || !watchedDedication) return null
    return findFirstWeeklyOverload(
      watchedStartDate,
      watchedEndDate,
      watchedDedication,
      workloadItems,
      editingId,
    )
  }, [workloadItems, watchedStartDate, watchedEndDate, watchedDedication, editingId])

  const remainingCapacity =
    peakLoad && !projectedOverload
      ? Math.max(0, 100 - peakLoad.allocatedPercent - (watchedDedication || 0))
      : null

  const personSelectPlaceholder = people.isLoading
    ? t('common.loading')
    : people.isError
      ? t('common.loading')
      : people.data?.items.length === 0
        ? t('empty.generic')
        : t('common.select')

  const plannedCount =
    allocations.data?.items.filter((allocation) => allocation.status === 'Planned').length ?? 0
  const confirmedCount =
    allocations.data?.items.filter((allocation) => allocation.status === 'Confirmed').length ?? 0

  return (
    <div>
      <PageHeader
        title={t('pages.allocations.title')}
        description={t('pages.allocations.description')}
        hideTitle
      />

      <PageGuide
        title={t('guide.howToRead')}
        steps={[tp('alloc.guide.step1'), tp('alloc.guide.step2'), tp('alloc.guide.step3')]}
      />

      {allocations.isSuccess && (
        <PageHero
          label={tp('alloc.hero.label')}
          value={allocations.data.totalCount}
          hint={tp('alloc.hero.hint')}
          metrics={
            <>
              <PageHeroMetric label={tp('alloc.hero.planned')} value={plannedCount} />
              <PageHeroMetric label={tp('alloc.hero.confirmed')} value={confirmedCount} />
            </>
          }
        />
      )}

      {prefilledFromMatching && !editingId && (
        <div className="mb-4 ias-alert-info rounded-lg px-4 py-3 text-sm">{t('alloc.prefillBanner')}</div>
      )}

      <PageSection
        title={editingId ? t('common.edit') : tp('alloc.new')}
        titleHint={tp('alloc.rn001Tip')}
        className="mb-6"
      >
        {!isTenantValid && (
          <p className="mb-3 text-sm ias-text-warning">{tp('alloc.tenantRequired')}</p>
        )}
        {(people.isError || projects.isError) && (
          <div className="mb-3">
            <Alert
              message={
                getErrorMessage(people.error) ||
                getErrorMessage(projects.error) ||
                tp('alloc.loadListsError')
              }
            />
          </div>
        )}
        <form onSubmit={onSubmit} className={formRowClass}>
          <FormField label={tp('common.person')} error={form.formState.errors.personId?.message}>
            <select
              {...form.register('personId')}
              className={inputClass}
              disabled={people.isLoading || people.isError}
            >
              <option value="">{personSelectPlaceholder}</option>
              {people.data?.items.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.name}
                </option>
              ))}
            </select>
          </FormField>
          <FormField label={t('common.project')} error={form.formState.errors.projectId?.message}>
            <select
              {...form.register('projectId')}
              className={inputClass}
              disabled={projects.isLoading || projects.isError}
            >
              <option value="">{personSelectPlaceholder}</option>
              {projects.data?.items.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.name}
                </option>
              ))}
            </select>
          </FormField>
          <FormField label={t('common.role')} error={form.formState.errors.role?.message}>
            <input {...form.register('role')} className={inputClass} />
          </FormField>
          <FormField
            label={t('common.dedication')}
            error={form.formState.errors.dedicationPercent?.message}
          >
            <input
              type="number"
              min={1}
              max={100}
              {...form.register('dedicationPercent', { valueAsNumber: true })}
              className={`w-20 ${inputClass}`}
            />
          </FormField>
          <FormField label={t('common.startDate')} error={form.formState.errors.startDate?.message}>
            <input type="date" {...form.register('startDate')} className={inputClass} />
          </FormField>
          <FormField label={t('common.endDate')} error={form.formState.errors.endDate?.message}>
            <input type="date" {...form.register('endDate')} className={inputClass} />
          </FormField>
          <FormField label={t('common.status')}>
            <select {...form.register('status')} className={inputClass}>
              {Object.entries(allocationStatusLabels).map(([k, v]) => (
                <option key={k} value={k}>
                  {v}
                </option>
              ))}
            </select>
          </FormField>
          <button
            type="submit"
            disabled={
              !isTenantValid ||
              create.isPending ||
              update.isPending ||
              !!projectedOverload
            }
            className={btnPrimaryClass}
          >
            {editingId ? t('common.save') : t('common.add')}
          </button>
          {editingId && (
            <button type="button" onClick={resetForm} className={btnGhostClass}>
              {t('common.cancel')}
            </button>
          )}
        </form>

        {watchedPersonId && watchedStartDate && watchedEndDate && (
          <div className="mt-4 rounded-lg border border-[var(--ias-border)] ias-panel-inset px-4 py-3 text-sm">
            <p className="mb-2 font-medium ias-text">{tp('alloc.workload.title')}</p>
            {personAllocations.isLoading && (
              <p className="ias-text-muted">{t('common.loading')}</p>
            )}
            {personAllocations.isSuccess && overlappingAllocations.length === 0 && (
              <p className="ias-text-muted">{tp('alloc.workload.empty')}</p>
            )}
            {personAllocations.isSuccess && peakLoad && (
              <p className="ias-text-muted">
                {tp('alloc.workload.peak', {
                  start: peakLoad.weekStart,
                  end: peakLoad.weekEnd,
                  allocated: String(peakLoad.allocatedPercent),
                })}
              </p>
            )}
            {remainingCapacity !== null && !projectedOverload && (
              <p className="mt-1 ias-text-muted">
                {tp('alloc.workload.remaining', { remaining: String(remainingCapacity) })}
              </p>
            )}
            {projectedOverload && (
              <div className="mt-2 rounded-lg ias-alert-warning px-3 py-2 text-sm">
                <p>
                  {tp('alloc.workload.overload', {
                    start: projectedOverload.weekStart,
                    end: projectedOverload.weekEnd,
                    existing: String(projectedOverload.existingPercent),
                    requested: String(projectedOverload.requestedPercent),
                    total: String(projectedOverload.totalPercent),
                  })}
                </p>
                <Link to="/conflicts" className="mt-1 inline-block ias-link text-xs">
                  {t('pages.conflicts.title')}
                </Link>
              </div>
            )}
            {overlappingAllocations.length > 0 && (
              <ul className="mt-3 space-y-1">
                <li className="text-xs ias-text-subtle">{tp('alloc.workload.existing')}</li>
                {overlappingAllocations.map((allocation) => (
                  <li key={allocation.id} className="flex flex-wrap gap-x-2 gap-y-1 ias-text">
                    <span className="font-medium">{allocation.projectName}</span>
                    <span className="ias-text-muted">
                      {allocation.dedicationPercent}% · {allocation.startDate} → {allocation.endDate}
                    </span>
                    <StatusBadge
                      label={allocationStatusLabels[allocation.status]}
                      status={allocation.status}
                    />
                  </li>
                ))}
              </ul>
            )}
            <p className="mt-3 text-xs ias-text-subtle">{tp('alloc.workload.closedNote')}</p>
          </div>
        )}

        {formError && (
          <div className="mt-3">
            <Alert message={formError} />
          </div>
        )}
      </PageSection>

      {allocations.isLoading && <LoadingState />}
      {allocations.isError && <Alert message={getErrorMessage(allocations.error)} />}
      {allocations.isSuccess && allocations.data.items.length === 0 && (
        <EmptyState message={tp('empty.allocations')} />
      )}
      {allocations.isSuccess && allocations.data.items.length > 0 && (
        <DataTable>
          <DataTableHead>
            <tr>
              <th>{t('nav.people')}</th>
              <th>{t('nav.projects')}</th>
              <th>{t('common.role')}</th>
              <th>%</th>
              <th>
                {t('common.from')} → {t('common.to')}
              </th>
              <th>{t('common.status')}</th>
              <th>{t('common.actions')}</th>
            </tr>
          </DataTableHead>
          <DataTableBody>
            {allocations.data.items.map((a) => (
              <tr key={a.id}>
                <td>
                  <TableCellIdentity title={a.personName} />
                </td>
                <td>
                  <TableCellIdentity title={a.projectName} />
                </td>
                <td>
                  <TableCellChip label={a.role} tone="low" />
                </td>
                <td>
                  <TableCellMetric
                    value={a.dedicationPercent}
                    unit="%"
                    tone={dedicationTone(a.dedicationPercent)}
                    showBar
                  />
                </td>
                <td>
                  <TableCellPeriod start={a.startDate} end={a.endDate} />
                </td>
                <td>
                  <StatusBadge label={allocationStatusLabels[a.status]} status={a.status} />
                </td>
                <td>
                  <TableActions
                    onEdit={() => setEditingId(a.id)}
                    onDelete={() => {
                      if (window.confirm(t('common.confirmDelete'))) remove.mutate(a.id)
                    }}
                    deleteDisabled={remove.isPending}
                  />
                </td>
              </tr>
            ))}
          </DataTableBody>
        </DataTable>
      )}
    </div>
  )
}
