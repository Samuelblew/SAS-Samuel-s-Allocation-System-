import { zodResolver } from '@hookform/resolvers/zod'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Fragment, useEffect, useMemo, useState } from 'react'
import { useForm } from 'react-hook-form'
import { Alert } from '../components/Alert'
import { DataTable, DataTableBody, DataTableHead } from '../components/DataTable'
import { EmptyState } from '../components/EmptyState'
import { FormDrawer } from '../components/forms/FormDrawer'
import { FormField } from '../components/forms/FormField'
import { LoadingState } from '../components/LoadingState'
import { PageHeader } from '../components/PageHeader'
import { PageGuide } from '../components/page/PageGuide'
import { PageHero, PageHeroMetric } from '../components/page/PageHero'
import { PageSection } from '../components/page/PageSection'
import { StatusBadge } from '../components/StatusBadge'
import { TableActions } from '../components/TableActions'
import { ToneBadge } from '../components/ToneBadge'
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
import { getErrorMessage } from '../lib/errors'
import {
  clientFormSchema,
  projectFormSchema,
  toProjectPayload,
  type ClientFormValues,
  type ProjectFormValues,
} from '../lib/schemas/project'
import { btnGhostClass, btnPrimaryClass, formRowClass, inputClass } from '../lib/ui'
import { dedicationTone, priorityTone } from '../lib/tableDisplay'
import type { AllocationListItem, ClientListItem, Paged, Project, ProjectListItem } from '../lib/types'

const defaultClientValues: ClientFormValues = { name: '' }

const defaultProjectValues: ProjectFormValues = {
  clientId: '',
  name: '',
  status: 'InProgress',
  priority: 'Medium',
  startDate: '',
  endDate: '',
}

export function ProjectsPage() {
  const { t } = useLocale()
  const { projectPriority: projectPriorityLabels, projectStatus: projectStatusLabels, allocationStatus: allocationStatusLabels } = useDomainLabels()
  const { isTenantValid } = useSettings()
  const opts = useApiOptions()
  const qc = useQueryClient()

  const [editingId, setEditingId] = useState<string | null>(null)
  const [expandedId, setExpandedId] = useState<string | null>(null)
  const [formError, setFormError] = useState<string | null>(null)

  const clientForm = useForm<ClientFormValues>({
    resolver: zodResolver(clientFormSchema),
    defaultValues: defaultClientValues,
  })

  const projectForm = useForm<ProjectFormValues>({
    resolver: zodResolver(projectFormSchema),
    defaultValues: defaultProjectValues,
  })

  const clients = useQuery({
    queryKey: ['clients', opts.tenantId],
    enabled: isTenantValid,
    queryFn: () => api.get<Paged<ClientListItem>>('/api/v1/clients?page=1&pageSize=100', opts),
  })

  const projects = useQuery({
    queryKey: ['projects', opts.tenantId],
    enabled: isTenantValid,
    queryFn: () => api.get<Paged<ProjectListItem>>('/api/v1/projects?page=1&pageSize=100', opts),
  })

  const allocations = useQuery({
    queryKey: ['allocations', opts.tenantId],
    enabled: isTenantValid,
    queryFn: () =>
      api.get<Paged<AllocationListItem>>(`/api/v1/allocations?page=1&pageSize=${LIST_PAGE_SIZE}`, opts),
  })

  const allocationsByProject = useMemo(() => {
    const map = new Map<string, AllocationListItem[]>()
    for (const allocation of allocations.data?.items ?? []) {
      if (allocation.status === 'Closed') continue
      const current = map.get(allocation.projectId) ?? []
      current.push(allocation)
      map.set(allocation.projectId, current)
    }
    for (const [projectId, items] of map) {
      map.set(
        projectId,
        [...items].sort((a, b) => a.personName.localeCompare(b.personName)),
      )
    }
    return map
  }, [allocations.data])

  const editingProject = useQuery({
    queryKey: ['project', opts.tenantId, editingId],
    enabled: isTenantValid && !!editingId,
    queryFn: () => api.get<Project>(`/api/v1/projects/${editingId}`, opts),
  })

  useEffect(() => {
    if (!editingProject.data) return
    const p = editingProject.data
    projectForm.reset({
      clientId: p.clientId,
      name: p.name,
      status: p.status,
      priority: p.priority,
      startDate: p.startDate ?? '',
      endDate: p.endDate ?? '',
    })
  }, [editingProject.data, projectForm])

  function resetProjectForm() {
    projectForm.reset(defaultProjectValues)
    setEditingId(null)
    setFormError(null)
  }

  const createClient = useMutation({
    mutationFn: (body: { name: string; notes: string | null }) =>
      api.post<{ id: string }>('/api/v1/clients', body, opts),
    onSuccess: (data) => {
      projectForm.setValue('clientId', data.id)
      clientForm.reset(defaultClientValues)
      void qc.invalidateQueries({ queryKey: ['clients'] })
    },
    onError: (err: Error) => setFormError(getErrorMessage(err)),
  })

  const createProject = useMutation({
    mutationFn: (body: ReturnType<typeof toProjectPayload>) =>
      api.post<ProjectListItem>('/api/v1/projects', body, opts),
    onSuccess: () => {
      resetProjectForm()
      void qc.invalidateQueries({ queryKey: ['projects'] })
      void qc.invalidateQueries({ queryKey: ['dashboard-counts'] })
    },
    onError: (err: Error) => setFormError(getErrorMessage(err)),
  })

  const updateProject = useMutation({
    mutationFn: ({ id, body }: { id: string; body: ReturnType<typeof toProjectPayload> }) =>
      api.put<Project>(`/api/v1/projects/${id}`, body, opts),
    onSuccess: () => {
      resetProjectForm()
      void qc.invalidateQueries({ queryKey: ['projects'] })
      void qc.invalidateQueries({ queryKey: ['project'] })
      void qc.invalidateQueries({ queryKey: ['dashboard-counts'] })
    },
    onError: (err: Error) => setFormError(getErrorMessage(err)),
  })

  const removeClient = useMutation({
    mutationFn: (id: string) => api.delete(`/api/v1/clients/${id}`, opts),
    onSuccess: (_data, id) => {
      if (projectForm.getValues('clientId') === id) {
        projectForm.setValue('clientId', '')
      }
      setFormError(null)
      void qc.invalidateQueries({ queryKey: ['clients'] })
      void qc.invalidateQueries({ queryKey: ['projects'] })
      void qc.invalidateQueries({ queryKey: ['dashboard-counts'] })
    },
    onError: (err: Error) => setFormError(getErrorMessage(err)),
  })

  const removeProject = useMutation({
    mutationFn: (id: string) => api.delete(`/api/v1/projects/${id}`, opts),
    onSuccess: () => {
      if (editingId) resetProjectForm()
      void qc.invalidateQueries({ queryKey: ['projects'] })
      void qc.invalidateQueries({ queryKey: ['dashboard-counts'] })
    },
    onError: (err: Error) => setFormError(getErrorMessage(err)),
  })

  const onClientSubmit = clientForm.handleSubmit((values) => {
    createClient.mutate({ name: values.name.trim(), notes: null })
  })

  const onProjectSubmit = projectForm.handleSubmit((values) => {
    const body = toProjectPayload(values)
    if (editingId) {
      updateProject.mutate({ id: editingId, body })
    } else {
      createProject.mutate(body)
    }
  })

  const inProgressCount =
    projects.data?.items.filter((project) => project.status === 'InProgress').length ?? 0

  return (
    <div>
      <PageHeader title={t('pages.projects.title')} description={t('pages.projects.description')} hideTitle />

      <PageGuide
        title={t('guide.howToRead')}
        steps={[
          t('pages.projects.guide.step1'),
          t('pages.projects.guide.step2'),
          t('pages.projects.guide.step3'),
        ]}
      />

      {projects.isSuccess && (
        <PageHero
          label={t('pages.projects.hero.label')}
          value={projects.data.totalCount}
          hint={t('pages.projects.hero.hint')}
          metrics={
            <>
              <PageHeroMetric
                label={t('pages.projects.hero.clients')}
                value={clients.data?.totalCount ?? '—'}
              />
              <PageHeroMetric
                label={t('pages.projects.hero.inProgress')}
                value={inProgressCount}
              />
            </>
          }
        />
      )}

      <div className="mb-6 grid gap-4 lg:grid-cols-2">
        <PageSection title={t('form.projects.newClient')}>
          <form onSubmit={onClientSubmit} className={formRowClass}>
            <FormField label={t('common.name')} error={clientForm.formState.errors.name?.message} className="min-w-[200px] flex-1">
              <input {...clientForm.register('name')} className={inputClass} placeholder={t('common.client')} />
            </FormField>
            <button type="submit" disabled={!isTenantValid || createClient.isPending} className={btnGhostClass}>
              {t('form.projects.createClient')}
            </button>
          </form>

          {clients.isSuccess && clients.data.items.length > 0 && (
            <div className="mt-4">
              <FormDrawer
                title={t('form.projects.registeredClients')}
                summary={t('form.projects.clientCount', {
                  count: String(clients.data.items.length),
                })}
                preview={
                  <>
                    {clients.data.items.slice(0, 4).map((client) => (
                      <span key={client.id} className="ias-form-drawer-preview-chip">
                        {client.name}
                      </span>
                    ))}
                    {clients.data.items.length > 4 ? (
                      <span className="ias-form-drawer-preview-chip">
                        +{clients.data.items.length - 4}
                      </span>
                    ) : null}
                  </>
                }
              >
                <ul className="space-y-2">
                  {clients.data.items.map((client) => {
                    const canDelete = client.projectCount === 0

                    return (
                      <li
                        key={client.id}
                        className="flex items-center justify-between gap-3 rounded-lg ias-list-row px-3 py-2 text-sm"
                      >
                        <div className="min-w-0">
                          <p className="truncate font-medium ias-text">{client.name}</p>
                          <p className="text-xs ias-text-subtle">
                            {client.projectCount}{' '}
                            {client.projectCount === 1 ? t('common.project') : t('nav.projects')}
                          </p>
                        </div>
                        <button
                          type="button"
                          disabled={!canDelete || removeClient.isPending}
                          title={canDelete ? undefined : t('form.projects.clientHasProjects')}
                          onClick={() => {
                            if (!canDelete) return
                            if (window.confirm(t('common.confirmDelete'))) removeClient.mutate(client.id)
                          }}
                          className="ias-table-action-btn ias-table-action-btn--danger shrink-0 disabled:cursor-not-allowed disabled:opacity-40"
                        >
                          {t('common.delete')}
                        </button>
                      </li>
                    )
                  })}
                </ul>
              </FormDrawer>
            </div>
          )}
        </PageSection>

        <PageSection title={editingId ? t('form.projects.edit') : t('form.projects.new')}>
          <form onSubmit={onProjectSubmit} className={formRowClass}>
            <FormField label={t('common.client')} error={projectForm.formState.errors.clientId?.message}>
              <select {...projectForm.register('clientId')} className={inputClass}>
                <option value="">{t('common.select')}</option>
                {clients.data?.items.map((c) => (
                  <option key={c.id} value={c.id}>
                    {c.name}
                  </option>
                ))}
              </select>
            </FormField>
            <FormField label={t('common.name')} error={projectForm.formState.errors.name?.message}>
              <input {...projectForm.register('name')} className={inputClass} />
            </FormField>
            <FormField label={t('common.status')}>
              <select {...projectForm.register('status')} className={inputClass}>
                {Object.entries(projectStatusLabels).map(([k, v]) => (
                  <option key={k} value={k}>
                    {v}
                  </option>
                ))}
              </select>
            </FormField>
            <FormField label={t('common.priority')}>
              <select {...projectForm.register('priority')} className={inputClass}>
                {Object.entries(projectPriorityLabels).map(([k, v]) => (
                  <option key={k} value={k}>
                    {v}
                  </option>
                ))}
              </select>
            </FormField>
            <FormField label={t('common.startDate')}>
              <input type="date" {...projectForm.register('startDate')} className={inputClass} />
            </FormField>
            <FormField label={t('common.endDate')}>
              <input type="date" {...projectForm.register('endDate')} className={inputClass} />
            </FormField>
            <button
              type="submit"
              disabled={!isTenantValid || createProject.isPending || updateProject.isPending}
              className={btnPrimaryClass}
            >
              {editingId ? t('common.save') : t('form.projects.create')}
            </button>
            {editingId && (
              <button type="button" onClick={resetProjectForm} className={btnGhostClass}>
                {t('common.cancel')}
              </button>
            )}
          </form>
        </PageSection>
      </div>

      {formError && (
        <div className="mb-4">
          <Alert message={formError} />
        </div>
      )}

      {projects.isLoading && <LoadingState />}
      {projects.isError && <Alert message={getErrorMessage(projects.error)} />}
      {projects.isSuccess && projects.data.items.length === 0 && (
        <EmptyState message={t('empty.projects')} />
      )}
      {projects.isSuccess && projects.data.items.length > 0 && (
        <DataTable>
          <DataTableHead>
            <tr>
              <th>{t('common.project')}</th>
              <th>{t('common.client')}</th>
              <th>{t('common.status')}</th>
              <th>{t('common.priority')}</th>
              <th>{t('common.period')}</th>
              <th>{t('pages.projects.team')}</th>
              <th>{t('common.actions')}</th>
            </tr>
          </DataTableHead>
          <DataTableBody>
            {projects.data.items.map((p) => {
              const projectAllocations = allocationsByProject.get(p.id) ?? []
              const isExpanded = expandedId === p.id

              return (
                <Fragment key={p.id}>
                  <tr>
                    <td>
                      <TableCellIdentity title={p.name} />
                    </td>
                    <td>
                      <TableCellChip label={p.clientName} tone="low" />
                    </td>
                    <td>
                      <StatusBadge label={projectStatusLabels[p.status]} status={p.status} />
                    </td>
                    <td>
                      <ToneBadge label={projectPriorityLabels[p.priority]} tone={priorityTone(p.priority)} />
                    </td>
                    <td>
                      <TableCellPeriod start={p.startDate} end={p.endDate} />
                    </td>
                    <td>
                      <span
                        className={
                          projectAllocations.length > 0
                            ? 'ias-table-skill-count'
                            : 'ias-table-empty-value'
                        }
                      >
                        {t('pages.projects.teamCount', {
                          count: String(projectAllocations.length),
                        })}
                      </span>
                    </td>
                    <td>
                      <TableActions
                        onEdit={() => setEditingId(p.id)}
                        onDelete={() => {
                          if (window.confirm(t('common.confirmDelete'))) removeProject.mutate(p.id)
                        }}
                        deleteDisabled={removeProject.isPending}
                        leading={[
                          {
                            label: isExpanded ? t('common.hide') : t('common.details'),
                            onClick: () => setExpandedId(isExpanded ? null : p.id),
                          },
                        ]}
                      />
                    </td>
                  </tr>
                  {isExpanded && (
                    <tr className="ias-panel-inset">
                      <td colSpan={7} className="px-4 py-4">
                        {projectAllocations.length === 0 ? (
                          <p className="text-sm ias-text-muted">{t('pages.projects.noTeam')}</p>
                        ) : (
                          <ul className="space-y-2">
                            {projectAllocations.map((allocation) => (
                              <li
                                key={allocation.id}
                                className="grid gap-3 rounded-lg ias-list-row px-3 py-3 text-sm sm:grid-cols-2 lg:grid-cols-4 xl:grid-cols-5"
                              >
                                <div className="min-w-0">
                                  <p className="text-xs ias-text-subtle">{t('nav.people')}</p>
                                  <p className="font-medium ias-text">{allocation.personName}</p>
                                </div>
                                <div>
                                  <p className="text-xs ias-text-subtle">{t('common.role')}</p>
                                  <TableCellChip label={allocation.role} tone="low" />
                                </div>
                                <div>
                                  <p className="text-xs ias-text-subtle">{t('common.dedication')}</p>
                                  <TableCellMetric
                                    value={allocation.dedicationPercent}
                                    unit="%"
                                    tone={dedicationTone(allocation.dedicationPercent)}
                                    showBar
                                  />
                                </div>
                                <div>
                                  <p className="text-xs ias-text-subtle">{t('common.period')}</p>
                                  <TableCellPeriod
                                    start={allocation.startDate}
                                    end={allocation.endDate}
                                  />
                                </div>
                                <div>
                                  <p className="text-xs ias-text-subtle">{t('common.status')}</p>
                                  <StatusBadge
                                    label={allocationStatusLabels[allocation.status]}
                                    status={allocation.status}
                                  />
                                </div>
                              </li>
                            ))}
                          </ul>
                        )}
                      </td>
                    </tr>
                  )}
                </Fragment>
              )
            })}
          </DataTableBody>
        </DataTable>
      )}
    </div>
  )
}
