import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Fragment, useEffect, useState, type FormEvent } from 'react'
import { Alert } from '../components/Alert'
import { DataTable, DataTableBody, DataTableHead } from '../components/DataTable'
import { EmptyState } from '../components/EmptyState'
import { EnterpriseKpiCard, EnterpriseKpiStrip } from '../components/enterprise/EnterpriseKpiCard'
import { FormActionBar } from '../components/enterprise/FormActionBar'
import { FormGrid, FormGridField } from '../components/enterprise/FormGrid'
import { WorkspacePanel } from '../components/enterprise/WorkspacePanel'
import { FormField } from '../components/forms/FormField'
import { LoadingState } from '../components/LoadingState'
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
import { LIST_PAGE_SIZE, SENIORITY_OPTIONS } from '../lib/constants'
import { api } from '../lib/api'
import { getErrorMessage } from '../lib/errors'
import { btnGhostClass, btnPrimaryClass, chipClass, inputClass } from '../lib/ui'
import { dedicationTone, urgencyTone } from '../lib/tableDisplay'
import type {
  AllocationNeed,
  AllocationNeedCriticality,
  AllocationNeedStatus,
  AllocationNeedUrgency,
  Paged,
  PagedAllocationNeeds,
  ProjectListItem,
  Skill,
} from '../lib/types'

function todayIso(): string {
  return new Date().toISOString().slice(0, 10)
}

export function AllocationNeedsPage() {
  const { t } = useLocale()
  const {
    allocationNeedStatus: allocationNeedStatusLabels,
    allocationNeedUrgency: allocationNeedUrgencyLabels,
    allocationNeedCriticality: allocationNeedCriticalityLabels,
  } = useDomainLabels()
  const { isTenantValid } = useSettings()
  const opts = useApiOptions()
  const qc = useQueryClient()

  const [projectId, setProjectId] = useState('')
  const [role, setRole] = useState('Backend')
  const [seniority, setSeniority] = useState('Senior')
  const [dedication, setDedication] = useState('50')
  const [startDate, setStartDate] = useState(todayIso())
  const [endDate, setEndDate] = useState('')
  const [urgency, setUrgency] = useState<AllocationNeedUrgency>('Medium')
  const [criticality, setCriticality] = useState<AllocationNeedCriticality>('Medium')
  const [needStatus, setNeedStatus] = useState<AllocationNeedStatus>('Open')
  const [selectedSkillIds, setSelectedSkillIds] = useState<string[]>([])
  const [editingId, setEditingId] = useState<string | null>(null)
  const [expandedId, setExpandedId] = useState<string | null>(null)
  const [formError, setFormError] = useState<string | null>(null)

  function resetForm() {
    setProjectId('')
    setRole('Backend')
    setSeniority('Senior')
    setDedication('50')
    setStartDate(todayIso())
    setEndDate('')
    setUrgency('Medium')
    setCriticality('Medium')
    setNeedStatus('Open')
    setSelectedSkillIds([])
    setEditingId(null)
    setFormError(null)
  }

  const projects = useQuery({
    queryKey: ['projects', opts.tenantId],
    enabled: isTenantValid,
    queryFn: () =>
      api.get<Paged<ProjectListItem>>(`/api/v1/projects?page=1&pageSize=${LIST_PAGE_SIZE}`, opts),
  })

  const skills = useQuery({
    queryKey: ['skills', opts.tenantId],
    enabled: isTenantValid,
    queryFn: () => api.get<Paged<Skill>>(`/api/v1/skills?page=1&pageSize=${LIST_PAGE_SIZE}`, opts),
  })

  const needs = useQuery({
    queryKey: ['allocation-needs', opts.tenantId],
    enabled: isTenantValid,
    queryFn: () =>
      api.get<PagedAllocationNeeds>('/api/v1/allocation-needs?page=1&pageSize=100', opts),
  })

  const invalidateRelated = () => {
    void qc.invalidateQueries({ queryKey: ['allocation-needs'] })
    void qc.invalidateQueries({ queryKey: ['capacity-understaffed'] })
    void qc.invalidateQueries({ queryKey: ['capacity-future-gaps'] })
    void qc.invalidateQueries({ queryKey: ['dashboard-counts'] })
  }

  const editingNeed = useQuery({
    queryKey: ['allocation-need', opts.tenantId, editingId],
    enabled: isTenantValid && !!editingId,
    queryFn: () => api.get<AllocationNeed>(`/api/v1/allocation-needs/${editingId}`, opts),
  })

  useEffect(() => {
    if (!editingNeed.data) return
    const n = editingNeed.data
    setProjectId(n.projectId)
    setRole(n.role)
    setSeniority(n.expectedSeniority ?? '')
    setDedication(String(n.dedicationPercent))
    setStartDate(n.startDate ?? todayIso())
    setEndDate(n.endDate ?? '')
    setUrgency(n.urgency)
    setCriticality(n.criticality)
    setNeedStatus(n.status)
    setSelectedSkillIds(n.requiredSkillIds)
  }, [editingNeed.data])

  const create = useMutation({
    mutationFn: (body: Record<string, unknown>) =>
      api.post<AllocationNeed>('/api/v1/allocation-needs', body, opts),
    onSuccess: () => {
      setFormError(null)
      invalidateRelated()
    },
    onError: (err: Error) => {
      setFormError(getErrorMessage(err))
    },
  })

  const update = useMutation({
    mutationFn: ({ id, body }: { id: string; body: Record<string, unknown> }) =>
      api.put<AllocationNeed>(`/api/v1/allocation-needs/${id}`, body, opts),
    onSuccess: () => {
      setFormError(null)
      resetForm()
      invalidateRelated()
    },
    onError: (err: Error) => {
      setFormError(getErrorMessage(err))
    },
  })

  const remove = useMutation({
    mutationFn: (id: string) => api.delete(`/api/v1/allocation-needs/${id}`, opts),
    onSuccess: () => {
      if (editingId) resetForm()
      invalidateRelated()
    },
  })

  function toggleSkill(skillId: string) {
    setSelectedSkillIds((prev) =>
      prev.includes(skillId) ? prev.filter((id) => id !== skillId) : [...prev, skillId],
    )
  }

  function buildBody(status: AllocationNeedStatus = needStatus) {
    return {
      projectId,
      role: role.trim(),
      expectedSeniority: seniority.trim() || null,
      requiredSkillIds: selectedSkillIds,
      desiredSkillIds: [],
      dedicationPercent: Number(dedication),
      startDate: startDate || null,
      endDate: endDate || null,
      urgency,
      criticality,
      status,
    }
  }

  function onSubmit(e: FormEvent) {
    e.preventDefault()
    if (editingId) {
      update.mutate({
        id: editingId,
        body: buildBody(),
      })
    } else {
      create.mutate(buildBody())
    }
  }

  const openCount = needs.data?.items.filter((need) => need.status === 'Open').length ?? 0
  const partialCount =
    needs.data?.items.filter((need) => need.status === 'PartiallyFilled').length ?? 0
  const activeTotal = openCount + partialCount

  return (
    <div className="ias-page-stack">
      {needs.isSuccess && (
        <EnterpriseKpiStrip>
          <EnterpriseKpiCard
            label={t('pages.needs.hero.open')}
            value={openCount}
            description={t('pages.needs.kpi.openDesc')}
          />
          <EnterpriseKpiCard
            label={t('pages.needs.hero.partial')}
            value={partialCount}
            description={t('pages.needs.kpi.partialDesc')}
          />
          <EnterpriseKpiCard
            label={t('pages.needs.kpi.total')}
            value={activeTotal}
            description={t('pages.needs.kpi.totalDesc')}
          />
        </EnterpriseKpiStrip>
      )}

      <WorkspacePanel
        title={editingId ? t('form.needs.editBanner') : t('pages.needs.title')}
        meta={
          editingId ? (
            <button type="button" onClick={resetForm} className="ias-workspace-panel__meta-link">
              {t('common.cancel')}
            </button>
          ) : undefined
        }
      >
        <form onSubmit={onSubmit} className="ias-page-form">
          <FormGrid>
            <FormGridField span={3}>
              <FormField label={t('common.project')}>
                <select
                  value={projectId}
                  onChange={(e) => setProjectId(e.target.value)}
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
              </FormField>
            </FormGridField>

            <FormGridField span={2}>
              <FormField label={t('common.role')}>
                <input value={role} onChange={(e) => setRole(e.target.value)} className={inputClass} />
              </FormField>
            </FormGridField>

            <FormGridField span={1}>
              <FormField label={t('common.seniority')}>
                <select
                  value={seniority}
                  onChange={(e) => setSeniority(e.target.value)}
                  className={inputClass}
                >
                  <option value="">{t('common.none')}</option>
                  {SENIORITY_OPTIONS.map((s) => (
                    <option key={s} value={s}>
                      {s}
                    </option>
                  ))}
                </select>
              </FormField>
            </FormGridField>

            <FormGridField span={1}>
              <FormField label={t('common.dedication')}>
                <input
                  type="number"
                  min={1}
                  max={100}
                  value={dedication}
                  onChange={(e) => setDedication(e.target.value)}
                  className={inputClass}
                />
              </FormField>
            </FormGridField>

            <FormGridField span={2}>
              <FormField label={t('common.startDate')}>
                <input
                  type="date"
                  value={startDate}
                  onChange={(e) => setStartDate(e.target.value)}
                  className={inputClass}
                />
              </FormField>
            </FormGridField>

            <FormGridField span={2}>
              <FormField label={t('common.endDate')}>
                <input
                  type="date"
                  value={endDate}
                  onChange={(e) => setEndDate(e.target.value)}
                  className={inputClass}
                />
              </FormField>
            </FormGridField>

            <FormGridField span={1}>
              <FormField label={t('common.urgency')}>
                <select
                  value={urgency}
                  onChange={(e) => setUrgency(e.target.value as AllocationNeedUrgency)}
                  className={inputClass}
                >
                  {Object.entries(allocationNeedUrgencyLabels).map(([k, v]) => (
                    <option key={k} value={k}>
                      {v}
                    </option>
                  ))}
                </select>
              </FormField>
            </FormGridField>

            <FormGridField span={1}>
              <FormField label={t('common.criticality')}>
                <select
                  value={criticality}
                  onChange={(e) => setCriticality(e.target.value as AllocationNeedCriticality)}
                  className={inputClass}
                >
                  {Object.entries(allocationNeedCriticalityLabels).map(([k, v]) => (
                    <option key={k} value={k}>
                      {v}
                    </option>
                  ))}
                </select>
              </FormField>
            </FormGridField>

            <FormGridField span={1}>
              <FormField label={t('common.status')}>
                <select
                  value={needStatus}
                  onChange={(e) => setNeedStatus(e.target.value as AllocationNeedStatus)}
                  className={inputClass}
                >
                  {Object.entries(allocationNeedStatusLabels).map(([k, v]) => (
                    <option key={k} value={k}>
                      {v}
                    </option>
                  ))}
                </select>
              </FormField>
            </FormGridField>

            {skills.data && skills.data.items.length > 0 && (
              <FormGridField span={6}>
                <fieldset className="ias-skills-fieldset">
                  <legend className="ias-skills-fieldset__legend">
                    {t('form.needs.requiredSkills')}
                    <span className="ias-skills-fieldset__count">
                      {selectedSkillIds.length > 0
                        ? t('form.needs.skillsSelected', { count: String(selectedSkillIds.length) })
                        : t('form.needs.noRequiredSkills')}
                    </span>
                  </legend>
                  <div className="ias-skills-fieldset__chips">
                    {skills.data.items.map((s) => (
                      <label key={s.id} className={`${chipClass} cursor-pointer gap-2`}>
                        <input
                          type="checkbox"
                          checked={selectedSkillIds.includes(s.id)}
                          onChange={() => toggleSkill(s.id)}
                          className="rounded border-[var(--ias-border-strong)]"
                        />
                        {s.name}
                      </label>
                    ))}
                  </div>
                </fieldset>
              </FormGridField>
            )}
          </FormGrid>

          {formError && <Alert message={formError} />}

          <FormActionBar>
            <button
              type="submit"
              disabled={!isTenantValid || create.isPending || update.isPending}
              className={btnPrimaryClass}
            >
              {editingId ? t('form.needs.saveChanges') : t('form.needs.create')}
            </button>
            {editingId && (
              <button type="button" onClick={resetForm} className={btnGhostClass}>
                {t('common.cancel')}
              </button>
            )}
          </FormActionBar>
        </form>
      </WorkspacePanel>

      <WorkspacePanel flush>
        {needs.isLoading && <LoadingState />}
        {needs.isError && <Alert message={getErrorMessage(needs.error)} />}
        {needs.isSuccess && needs.data.items.length === 0 && (
          <EmptyState message={t('empty.needs')} />
        )}
        {needs.isSuccess && needs.data.items.length > 0 && (
          <DataTable>
            <DataTableHead>
              <tr>
                <th>{t('common.project')}</th>
                <th>{t('common.role')}</th>
                <th>{t('common.seniority')}</th>
                <th>%</th>
                <th>{t('common.period')}</th>
                <th>{t('common.status')}</th>
                <th>{t('common.urgency')}</th>
                <th>{t('common.actions')}</th>
              </tr>
            </DataTableHead>
            <DataTableBody>
              {needs.data.items.map((n) => {
                const skillNames = n.requiredSkillIds
                  .map((id) => skills.data?.items.find((s) => s.id === id)?.name)
                  .filter(Boolean)
                const isExpanded = expandedId === n.id

                return (
                  <Fragment key={n.id}>
                    <tr>
                      <td>
                        <TableCellIdentity title={n.projectName} subtitle={n.role} />
                      </td>
                      <td>
                        <TableCellChip label={n.role} tone="low" />
                      </td>
                      <td>
                        <TableCellChip label={n.expectedSeniority} />
                      </td>
                      <td>
                        <TableCellMetric
                          value={n.dedicationPercent}
                          unit="%"
                          tone={dedicationTone(n.dedicationPercent)}
                          showBar
                        />
                      </td>
                      <td>
                        <TableCellPeriod start={n.startDate} end={n.endDate} />
                      </td>
                      <td>
                        <StatusBadge
                          label={allocationNeedStatusLabels[n.status] ?? n.status}
                          status={n.status}
                        />
                      </td>
                      <td>
                        <ToneBadge
                          label={allocationNeedUrgencyLabels[n.urgency] ?? n.urgency}
                          tone={urgencyTone(n.urgency)}
                        />
                      </td>
                      <td>
                        <TableActions
                          onEdit={() => {
                            setEditingId(n.id)
                            setNeedStatus(n.status)
                          }}
                          onDelete={() => remove.mutate(n.id)}
                          deleteDisabled={remove.isPending}
                          leading={[
                            {
                              label: isExpanded ? t('common.hide') : t('common.details'),
                              onClick: () => setExpandedId(isExpanded ? null : n.id),
                            },
                          ]}
                          extra={
                            n.status !== 'Filled'
                              ? [
                                  {
                                    label: 'Matching',
                                    href: `/matching?needId=${n.id}`,
                                    variant: 'accent',
                                  },
                                ]
                              : undefined
                          }
                        />
                      </td>
                    </tr>
                    {isExpanded && (
                      <tr className="ias-panel-inset p-3">
                        <td colSpan={8} className="px-3 py-2">
                          <div className="grid gap-2 text-sm sm:grid-cols-2 lg:grid-cols-4">
                            <div>
                              <p className="text-xs ias-text-subtle">{t('common.criticality')}</p>
                              <p className="ias-text-muted">
                                {allocationNeedCriticalityLabels[n.criticality] ?? n.criticality}
                              </p>
                            </div>
                            <div>
                              <p className="text-xs ias-text-subtle">{t('common.seniority')}</p>
                              <p className="ias-text-muted">{n.expectedSeniority ?? '—'}</p>
                            </div>
                            <div>
                              <p className="text-xs ias-text-subtle">{t('common.dedication')}</p>
                              <p className="ias-text-muted">{n.dedicationPercent}%</p>
                            </div>
                            <div>
                              <p className="text-xs ias-text-subtle">{t('common.period')}</p>
                              <p className="ias-text-muted">
                                {n.startDate ?? '—'} → {n.endDate ?? '—'}
                              </p>
                            </div>
                          </div>
                          <div className="mt-2">
                            <p className="text-xs ias-text-subtle">{t('form.needs.requiredSkills')}</p>
                            {skillNames.length > 0 ? (
                              <div className="mt-1 flex flex-wrap gap-1.5">
                                {skillNames.map((name) => (
                                  <span key={name} className={chipClass}>
                                    {name}
                                  </span>
                                ))}
                              </div>
                            ) : (
                              <p className="mt-1 ias-text-muted">{t('form.needs.noRequiredSkills')}</p>
                            )}
                          </div>
                        </td>
                      </tr>
                    )}
                  </Fragment>
                )
              })}
            </DataTableBody>
          </DataTable>
        )}
      </WorkspacePanel>
    </div>
  )
}
