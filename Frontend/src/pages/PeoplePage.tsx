import { zodResolver } from '@hookform/resolvers/zod'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useEffect, useState } from 'react'
import { useForm } from 'react-hook-form'
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
  TableCellSkillCount,
} from '../components/table/TableCells'
import { useDomainLabels, useLocale } from '../context/LocaleContext'
import { useApiOptions, useSettings } from '../context/SettingsContext'
import { LIST_PAGE_SIZE, SENIORITY_OPTIONS, SKILL_PROFICIENCY_LEVELS } from '../lib/constants'
import { api } from '../lib/api'
import { getErrorMessage } from '../lib/errors'
import {
  personFormSchema,
  personSkillFormSchema,
  toPersonPayload,
  type PersonFormValues,
  type PersonSkillFormValues,
} from '../lib/schemas/person'
import { btnGhostClass, btnPrimaryClass, formRowClass, inputClass } from '../lib/ui'
import { dedicationTone } from '../lib/tableDisplay'
import type { Paged, Person, PersonListItem, Skill } from '../lib/types'

const defaultPersonValues: PersonFormValues = {
  name: '',
  jobTitle: '',
  seniority: '',
  weeklyCapacityHours: 40,
  status: 'Active',
}

const defaultSkillValues: PersonSkillFormValues = {
  skillId: '',
  level: 'Intermediate',
}

export function PeoplePage() {
  const { t } = useLocale()
  const { personStatus: personStatusLabels, skillProficiency: skillProficiencyLabels } = useDomainLabels()
  const { isTenantValid } = useSettings()
  const opts = useApiOptions()
  const qc = useQueryClient()

  const [editingId, setEditingId] = useState<string | null>(null)
  const [skillsPersonId, setSkillsPersonId] = useState<string | null>(null)
  const [formError, setFormError] = useState<string | null>(null)

  const personForm = useForm<PersonFormValues>({
    resolver: zodResolver(personFormSchema),
    defaultValues: defaultPersonValues,
  })

  const skillForm = useForm<PersonSkillFormValues>({
    resolver: zodResolver(personSkillFormSchema),
    defaultValues: defaultSkillValues,
  })

  const query = useQuery({
    queryKey: ['people', opts.tenantId],
    enabled: isTenantValid,
    queryFn: () => api.get<Paged<PersonListItem>>('/api/v1/people?page=1&pageSize=100', opts),
  })

  const skills = useQuery({
    queryKey: ['skills', opts.tenantId],
    enabled: isTenantValid,
    queryFn: () => api.get<Paged<Skill>>(`/api/v1/skills?page=1&pageSize=${LIST_PAGE_SIZE}`, opts),
  })

  const personDetail = useQuery({
    queryKey: ['person', opts.tenantId, skillsPersonId],
    enabled: isTenantValid && !!skillsPersonId,
    queryFn: () => api.get<Person>(`/api/v1/people/${skillsPersonId}`, opts),
  })

  const editingPerson = useQuery({
    queryKey: ['person', opts.tenantId, editingId],
    enabled: isTenantValid && !!editingId,
    queryFn: () => api.get<Person>(`/api/v1/people/${editingId}`, opts),
  })

  useEffect(() => {
    if (!editingPerson.data) return
    const p = editingPerson.data
    personForm.reset({
      name: p.name,
      jobTitle: p.jobTitle ?? '',
      seniority: p.seniority ?? '',
      weeklyCapacityHours: p.weeklyCapacityHours,
      status: p.status,
    })
  }, [editingPerson.data, personForm])

  function resetPersonForm() {
    personForm.reset(defaultPersonValues)
    setEditingId(null)
    setFormError(null)
  }

  const invalidatePeople = () => {
    void qc.invalidateQueries({ queryKey: ['people'] })
    void qc.invalidateQueries({ queryKey: ['person'] })
    void qc.invalidateQueries({ queryKey: ['dashboard-counts'] })
  }

  const create = useMutation({
    mutationFn: (body: ReturnType<typeof toPersonPayload>) =>
      api.post<PersonListItem>('/api/v1/people', body, opts),
    onSuccess: () => {
      resetPersonForm()
      invalidatePeople()
    },
    onError: (err: Error) => setFormError(getErrorMessage(err)),
  })

  const update = useMutation({
    mutationFn: ({ id, body }: { id: string; body: ReturnType<typeof toPersonPayload> }) =>
      api.put<Person>(`/api/v1/people/${id}`, body, opts),
    onSuccess: () => {
      resetPersonForm()
      invalidatePeople()
    },
    onError: (err: Error) => setFormError(getErrorMessage(err)),
  })

  const remove = useMutation({
    mutationFn: (id: string) => api.delete(`/api/v1/people/${id}`, opts),
    onSuccess: () => {
      if (editingId) resetPersonForm()
      if (skillsPersonId) setSkillsPersonId(null)
      invalidatePeople()
    },
    onError: (err: Error) => setFormError(getErrorMessage(err)),
  })

  const addSkill = useMutation({
    mutationFn: ({
      personId,
      body,
    }: {
      personId: string
      body: { skillId: string; level: string; lastUsedAt: null; notes: null }
    }) => api.post(`/api/v1/people/${personId}/skills`, body, opts),
    onSuccess: () => {
      skillForm.reset(defaultSkillValues)
      setFormError(null)
      invalidatePeople()
    },
    onError: (err: Error) => setFormError(getErrorMessage(err)),
  })

  const removeSkill = useMutation({
    mutationFn: ({ personId, personSkillId }: { personId: string; personSkillId: string }) =>
      api.delete(`/api/v1/people/${personId}/skills/${personSkillId}`, opts),
    onSuccess: () => invalidatePeople(),
    onError: (err: Error) => setFormError(getErrorMessage(err)),
  })

  const onPersonSubmit = personForm.handleSubmit((values) => {
    const body = toPersonPayload(values)
    if (editingId) {
      update.mutate({ id: editingId, body })
    } else {
      create.mutate(body)
    }
  })

  const onAddSkill = skillForm.handleSubmit((values) => {
    if (!skillsPersonId) return
    addSkill.mutate({
      personId: skillsPersonId,
      body: { skillId: values.skillId, level: values.level, lastUsedAt: null, notes: null },
    })
  })

  const activeCount =
    query.data?.items.filter((person) => person.status === 'Active' || person.status === 'Contractor')
      .length ?? 0

  return (
    <div>
      <PageHeader title={t('pages.people.title')} description={t('pages.people.description')} hideTitle />

      <PageGuide
        title={t('guide.howToRead')}
        steps={[
          t('pages.people.guide.step1'),
          t('pages.people.guide.step2'),
          t('pages.people.guide.step3'),
        ]}
      />

      {query.isSuccess && (
        <PageHero
          label={t('pages.people.hero.label')}
          value={query.data.totalCount}
          hint={t('pages.people.hero.hint')}
          metrics={
            <PageHeroMetric label={t('pages.people.hero.active')} value={activeCount} valueClassName="ias-text-success" />
          }
        />
      )}

      <PageSection
        title={editingId ? t('form.people.edit') : t('form.people.new')}
        className="mb-6"
      >
        <form onSubmit={onPersonSubmit} className={formRowClass}>
          <FormField label={t('common.name')} error={personForm.formState.errors.name?.message}>
            <input {...personForm.register('name')} className={inputClass} />
          </FormField>
          <FormField label={t('common.jobTitle')}>
            <input {...personForm.register('jobTitle')} className={inputClass} />
          </FormField>
          <FormField label={t('common.seniority')}>
            <select {...personForm.register('seniority')} className={inputClass}>
              <option value="">{t('common.none')}</option>
              {SENIORITY_OPTIONS.map((s) => (
                <option key={s} value={s}>
                  {s}
                </option>
              ))}
            </select>
          </FormField>
          <FormField label={t('common.weeklyHours')} error={personForm.formState.errors.weeklyCapacityHours?.message}>
            <input
              type="number"
              min={1}
              {...personForm.register('weeklyCapacityHours', { valueAsNumber: true })}
              className={`w-24 ${inputClass}`}
            />
          </FormField>
          <FormField label={t('common.status')}>
            <select {...personForm.register('status')} className={inputClass}>
              {Object.entries(personStatusLabels).map(([k, v]) => (
                <option key={k} value={k}>
                  {v}
                </option>
              ))}
            </select>
          </FormField>
          <button
            type="submit"
            disabled={!isTenantValid || create.isPending || update.isPending}
            className={btnPrimaryClass}
          >
            {editingId ? t('common.save') : t('common.add')}
          </button>
          {editingId && (
            <button type="button" onClick={resetPersonForm} className={btnGhostClass}>
              {t('common.cancel')}
            </button>
          )}
        </form>
        {formError && (
          <div className="mt-3">
            <Alert message={formError} />
          </div>
        )}
      </PageSection>

      {skillsPersonId && personDetail.data && (
        <PageSection
          title={t('form.people.skillsOf', { name: personDetail.data.name })}
          className="mb-6"
        >
          <div className="mb-3 flex justify-end">
            <button type="button" onClick={() => setSkillsPersonId(null)} className="ias-table-action-btn">
              {t('common.close')}
            </button>
          </div>
          {personDetail.data.skills.length > 0 ? (
            <ul className="mb-4 space-y-2">
              {personDetail.data.skills.map((ps) => (
                <li
                  key={ps.id}
                  className="flex items-center justify-between rounded-lg ias-list-row px-3 py-2 text-sm"
                >
                  <span className="ias-text">
                    {ps.skillName}
                    <span className="ml-2 ias-text-subtle">
                      {skillProficiencyLabels[ps.level as keyof typeof skillProficiencyLabels] ?? ps.level}
                    </span>
                  </span>
                  <button
                    type="button"
                    onClick={() =>
                      removeSkill.mutate({ personId: skillsPersonId, personSkillId: ps.id })
                    }
                    className="text-xs ias-text-danger hover:ias-text-danger"
                  >
                    {t('common.remove')}
                  </button>
                </li>
              ))}
            </ul>
          ) : (
            <p className="mb-4 text-sm ias-text-subtle">{t('form.people.noSkills')}</p>
          )}
          <form onSubmit={onAddSkill} className={formRowClass}>
            <FormField label={t('common.skills')} error={skillForm.formState.errors.skillId?.message}>
              <select {...skillForm.register('skillId')} className={inputClass}>
                <option value="">{t('common.select')}</option>
                {skills.data?.items.map((s) => (
                  <option key={s.id} value={s.id}>
                    {s.name}
                  </option>
                ))}
              </select>
            </FormField>
            <FormField label={t('form.people.skillLevel')}>
              <select {...skillForm.register('level')} className={inputClass}>
                {SKILL_PROFICIENCY_LEVELS.map((l) => (
                  <option key={l} value={l}>
                    {skillProficiencyLabels[l]}
                  </option>
                ))}
              </select>
            </FormField>
            <button type="submit" disabled={addSkill.isPending} className={btnGhostClass}>
              {t('form.people.assignSkill')}
            </button>
          </form>
        </PageSection>
      )}

      {query.isLoading && <LoadingState />}
      {query.isError && <Alert message={getErrorMessage(query.error)} />}
      {query.isSuccess && query.data.items.length === 0 && <EmptyState message={t('empty.people')} />}
      {query.isSuccess && query.data.items.length > 0 && (
        <DataTable>
          <DataTableHead>
            <tr>
              <th>{t('common.name')}</th>
              <th>{t('common.jobTitle')}</th>
              <th>{t('common.seniority')}</th>
              <th>{t('common.weeklyHours')}</th>
              <th>{t('common.status')}</th>
              <th>{t('common.skills')}</th>
              <th>{t('common.actions')}</th>
            </tr>
          </DataTableHead>
          <DataTableBody>
            {query.data.items.map((p) => (
              <tr key={p.id}>
                <td>
                  <TableCellIdentity title={p.name} />
                </td>
                <td>
                  <TableCellChip label={p.jobTitle} />
                </td>
                <td>
                  <TableCellChip label={p.seniority} tone="neutral" />
                </td>
                <td>
                  <TableCellMetric
                    value={p.weeklyCapacityHours}
                    unit="h"
                    tone={dedicationTone(Math.min(100, (p.weeklyCapacityHours / 40) * 100))}
                    showBar
                    barMax={40}
                  />
                </td>
                <td>
                  <StatusBadge label={personStatusLabels[p.status]} status={p.status} />
                </td>
                <td>
                  <TableCellSkillCount count={p.skillCount} />
                </td>
                <td>
                  <TableActions
                    onEdit={() => setEditingId(p.id)}
                    onDelete={() => {
                      if (window.confirm(t('common.confirmDelete'))) remove.mutate(p.id)
                    }}
                    deleteDisabled={remove.isPending}
                    extra={[
                      {
                        label: t('common.skills'),
                        onClick: () => setSkillsPersonId(p.id),
                        variant: 'accent',
                      },
                    ]}
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
